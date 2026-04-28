import { useMutation, useQuery } from "@tanstack/react-query";
import { Sheet, SheetContent, SheetHeader, SheetTitle } from "@/components/ui/sheet";
import { api, type AgentDecisionResponse } from "@/lib/api";
import { useI18n } from "@/lib/i18n";
import { LoadingBlock, ErrorBlock } from "@/components/states";
import { ActionBadge, DecisionBadge, ProviderBadge, RiskBadge, SeverityBadge } from "@/components/badges";
import { Button } from "@/components/ui/button";
import { Info, Sparkles } from "lucide-react";

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

  const analyzeM = useMutation({
    mutationFn: () => api.analyzeScan(scanId!, lang),
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

            <div className="rounded-lg border bg-card">
              <div className="flex items-center justify-between border-b px-4 py-3">
                <h3 className="text-sm font-semibold">{t("agent.decision")}</h3>
                <Button
                  size="sm"
                  variant="outline"
                  onClick={() => analyzeM.mutate()}
                  disabled={analyzeM.isPending || !scanId}
                >
                  <Sparkles className="mr-1.5 h-3.5 w-3.5" />
                  {analyzeM.isPending ? t("agent.analyzing") : t("agent.analyze")}
                </Button>
              </div>
              <div className="p-4">
                {analyzeM.isPending && <LoadingBlock />}
                {analyzeM.isError && (
                  <ErrorBlock
                    error={analyzeM.error}
                    onRetry={() => analyzeM.mutate()}
                  />
                )}
                {analyzeM.data && <AgentDecisionView data={analyzeM.data} />}
                {!analyzeM.isPending && !analyzeM.data && !analyzeM.isError && (
                  <p className="text-xs text-muted-foreground">
                    {t("agent.analyze")}.
                  </p>
                )}
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
  const provider = (data.explanationProvider ?? "").toLowerCase();
  const isFallback = provider === "fallback" || provider === "rulebased";
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

      {isFallback && (
        <div className="flex items-start gap-2 rounded-md border border-amber-200 bg-amber-50/60 px-3 py-2 text-[11px] text-amber-800">
          <Info className="mt-0.5 h-3.5 w-3.5 shrink-0" />
          <span>{t("ai.fallbackHint")}</span>
        </div>
      )}

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

function AgentDecisionView({ data }: { data: AgentDecisionResponse }) {
  const { t } = useI18n();
  const actions = data.actions ?? [];
  const enrichment = data.enrichment;
  const provider = (enrichment?.provider ?? "").toLowerCase();
  const isRuleBased = provider === "rulebased" || provider === "fallback";
  const suggested = enrichment?.suggestedNextActions ?? [];

  return (
    <div className="space-y-4 text-sm">
      <div className="grid grid-cols-2 gap-3 sm:grid-cols-3">
        <Stat label={t("agent.severity")} value={<SeverityBadge severity={data.severity} />} />
        <Stat
          label={t("agent.requiresReview")}
          value={data.requiresHumanReview ? "Yes" : "No"}
        />
        <Stat
          label={t("agent.actions")}
          value={
            actions.length > 0 ? (
              <div className="flex flex-wrap gap-1">
                {actions.map((a, i) => (
                  <ActionBadge key={i} action={a} />
                ))}
              </div>
            ) : (
              "—"
            )
          }
        />
      </div>

      {data.reason && (
        <div>
          <div className="mb-1 text-[11px] uppercase tracking-wide text-muted-foreground">
            {t("agent.reason")}
          </div>
          <p className="rounded-md bg-muted/50 p-3 whitespace-pre-wrap">{data.reason}</p>
        </div>
      )}

      <div className="rounded-md border bg-background">
        <div className="border-b px-3 py-2 text-xs font-semibold uppercase tracking-wide text-muted-foreground">
          {t("agent.v2Title")}
        </div>
        <div className="p-3">
          {!enrichment ? (
            <p className="text-xs text-muted-foreground">{t("agent.noEnrichment")}</p>
          ) : (
            <div className="space-y-3">
              <div className="flex flex-wrap items-center gap-2">
                <span className="text-[11px] uppercase tracking-wide text-muted-foreground">
                  {t("agent.provider")}:
                </span>
                <ProviderBadge provider={enrichment.provider ?? "—"} />
                <span className="text-[11px] uppercase tracking-wide text-muted-foreground">
                  {t("agent.confidenceScore")}:
                </span>
                <span className="font-mono text-xs">
                  {typeof enrichment.confidenceScore === "number"
                    ? enrichment.confidenceScore.toFixed(2)
                    : "—"}
                </span>
              </div>

              {isRuleBased && (
                <div className="flex items-start gap-2 rounded-md border border-amber-200 bg-amber-50/60 px-3 py-2 text-[11px] text-amber-800">
                  <Info className="mt-0.5 h-3.5 w-3.5 shrink-0" />
                  <span>{t("agent.ruleBasedHint")}</span>
                </div>
              )}

              <div>
                <div className="mb-1 text-[11px] uppercase tracking-wide text-muted-foreground">
                  {t("agent.operatorSummary")}
                </div>
                <p className="rounded-md bg-muted/50 p-3 whitespace-pre-wrap">
                  {enrichment.operatorSummary ?? "—"}
                </p>
              </div>

              <div>
                <div className="mb-1 text-[11px] uppercase tracking-wide text-muted-foreground">
                  {t("agent.businessImpact")}
                </div>
                <p className="rounded-md bg-muted/50 p-3 whitespace-pre-wrap">
                  {enrichment.businessImpact ?? "—"}
                </p>
              </div>

              <div>
                <div className="mb-1.5 text-[11px] uppercase tracking-wide text-muted-foreground">
                  {t("agent.suggestedActions")}
                </div>
                {suggested.length > 0 ? (
                  <ul className="space-y-1.5">
                    {suggested.map((s, i) => (
                      <li
                        key={i}
                        className="flex items-start gap-2 rounded border bg-card px-3 py-2 text-xs"
                      >
                        <span className="mt-1 inline-block h-1.5 w-1.5 shrink-0 rounded-full bg-primary" />
                        <span>{s}</span>
                      </li>
                    ))}
                  </ul>
                ) : (
                  <p className="text-xs text-muted-foreground">—</p>
                )}
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}