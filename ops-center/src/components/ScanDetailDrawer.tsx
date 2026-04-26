import { useQuery } from "@tanstack/react-query";
import { Sheet, SheetContent, SheetHeader, SheetTitle } from "@/components/ui/sheet";
import { api } from "@/lib/api";
import { useI18n } from "@/lib/i18n";
import { LoadingBlock, ErrorBlock } from "@/components/states";
import { DecisionBadge, ProviderBadge, RiskBadge } from "@/components/badges";

interface Props {
  scanId: string | null;
  open: boolean;
  onOpenChange: (o: boolean) => void;
}

export function ScanDetailDrawer({ scanId, open, onOpenChange }: Props) {
  const { t, lang } = useI18n();

  const scanQ = useQuery({
    queryKey: ["scan", scanId],
    queryFn: () => api.scan(scanId!),
    enabled: !!scanId && open,
  });

  const riskQ = useQuery({
    queryKey: ["risk", scanId, lang],
    queryFn: () => api.risk(scanId!, lang),
    enabled: !!scanId && open,
  });

  return (
    <Sheet open={open} onOpenChange={onOpenChange}>
      <SheetContent className="w-full overflow-y-auto sm:max-w-xl">
        <SheetHeader>
          <SheetTitle>{t("drawer.title")}</SheetTitle>
        </SheetHeader>

        {scanQ.isLoading && <LoadingBlock />}
        {scanQ.isError && <ErrorBlock error={scanQ.error} onRetry={() => scanQ.refetch()} />}
        {scanQ.data && (
          <div className="mt-4 space-y-4">
            <div className="rounded-lg border bg-card p-4">
              <dl className="grid grid-cols-2 gap-x-4 gap-y-3 text-sm">
                <Field label={t("common.ticket")} value={<span className="font-mono text-xs">{scanQ.data.ticketCode}</span>} />
                <Field label={t("common.time")} value={scanQ.data.scannedAtUtc ? new Date(scanQ.data.scannedAtUtc).toLocaleString() : "—"} />
                <Field label={t("common.decision")} value={<DecisionBadge decision={scanQ.data.decision} />} />
                <Field label={t("common.reason")} value={scanQ.data.reasonCode ?? "—"} />
                <Field label={t("common.gate")} value={scanQ.data.gateId ?? "—"} />
                <Field label={t("common.device")} value={<span className="font-mono text-xs">{scanQ.data.deviceId ?? "—"}</span>} />
                <Field label={t("common.source")} value={scanQ.data.source ?? "—"} />
              </dl>
            </div>

            <div className="rounded-lg border bg-card">
              <div className="border-b px-4 py-3">
                <h3 className="text-sm font-semibold">{t("drawer.aiRisk")}</h3>
              </div>
              <div className="p-4">
                {riskQ.isLoading && <LoadingBlock />}
                {riskQ.isError && <ErrorBlock error={riskQ.error} onRetry={() => riskQ.refetch()} />}
                {riskQ.data && <RiskAnalysisView data={riskQ.data} />}
              </div>
            </div>
          </div>
        )}
      </SheetContent>
    </Sheet>
  );
}

function Field({ label, value }: { label: string; value: React.ReactNode }) {
  return (
    <div>
      <dt className="text-[11px] uppercase tracking-wide text-muted-foreground">{label}</dt>
      <dd className="mt-0.5 font-medium">{value}</dd>
    </div>
  );
}

export function RiskAnalysisView({ data }: { data: import("@/lib/api").RiskAnalysis }) {
  const { t } = useI18n();
  return (
    <div className="space-y-4 text-sm">
      <div className="grid grid-cols-2 gap-3 sm:grid-cols-4">
        <Stat label={t("drawer.score")} value={data.riskScore != null ? data.riskScore.toFixed(2) : "—"} />
        <Stat label={t("drawer.level")} value={<RiskBadge level={data.riskLevel} />} />
        <Stat label={t("drawer.action")} value={data.recommendedAction ?? "—"} />
        <Stat label={t("drawer.confidence")} value={data.explanationConfidence != null ? `${(data.explanationConfidence * 100).toFixed(0)}%` : "—"} />
      </div>

      <div className="flex flex-wrap items-center gap-2">
        <span className="text-xs text-muted-foreground">{t("drawer.provider")}:</span>
        <ProviderBadge provider={data.explanationProvider} />
      </div>

      {data.explanationSummary && (
        <div>
          <div className="mb-1 text-[11px] uppercase tracking-wide text-muted-foreground">
            {t("drawer.summary")}
          </div>
          <p className="rounded-md bg-muted/50 p-3">{data.explanationSummary}</p>
        </div>
      )}

      {data.riskExplanation && (
        <div>
          <div className="mb-1 text-[11px] uppercase tracking-wide text-muted-foreground">
            {t("drawer.explanation")}
          </div>
          <p className="rounded-md bg-muted/50 p-3 whitespace-pre-wrap">{data.riskExplanation}</p>
        </div>
      )}

      {data.riskSignals && data.riskSignals.length > 0 && (
        <div>
          <div className="mb-1.5 text-[11px] uppercase tracking-wide text-muted-foreground">
            {t("drawer.signals")}
          </div>
          <ul className="space-y-1.5">
            {data.riskSignals.map((sig, i) => {
              if (typeof sig === "string") {
                return (
                  <li key={i} className="rounded border bg-card px-3 py-2 text-xs">
                    {sig}
                  </li>
                );
              }
              return (
                <li key={i} className="flex items-start justify-between gap-2 rounded border bg-card px-3 py-2 text-xs">
                  <div>
                    <div className="font-medium">{sig.name ?? "Signal"}</div>
                    {sig.description && (
                      <div className="text-muted-foreground">{sig.description}</div>
                    )}
                  </div>
                  {sig.weight != null && (
                    <span className="rounded bg-muted px-1.5 py-0.5 font-mono">
                      {sig.weight.toFixed(2)}
                    </span>
                  )}
                </li>
              );
            })}
          </ul>
        </div>
      )}
    </div>
  );
}

function Stat({ label, value }: { label: string; value: React.ReactNode }) {
  return (
    <div className="rounded-md border bg-background p-3">
      <div className="text-[11px] uppercase tracking-wide text-muted-foreground">{label}</div>
      <div className="mt-1 font-semibold">{value}</div>
    </div>
  );
}