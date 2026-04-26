import { createFileRoute } from "@tanstack/react-router";
import { useState } from "react";
import { useMutation, useQuery } from "@tanstack/react-query";
import { Loader2, ScanLine, CheckCircle2, AlertCircle } from "lucide-react";
import { api, type Scan, type RiskAnalysis } from "@/lib/api";
import { useI18n } from "@/lib/i18n";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { DecisionBadge, RiskBadge, ProviderBadge } from "@/components/badges";

export const Route = createFileRoute("/manual-scan")({
  component: ManualScanPage,
});

type ValidateResult = Scan &
  RiskAnalysis & {
    accepted?: boolean;
    message?: string;
    scanAttemptId?: string;
  };

function ManualScanPage() {
  const { t, lang } = useI18n();
  const [ticketCode, setTicketCode] = useState("");
  const [deviceId, setDeviceId] = useState("");
  const [gateId, setGateId] = useState("");
  const [source, setSource] = useState("ops-center");

  const m = useMutation<ValidateResult, Error, void>({
    mutationFn: () =>
      api.validateScan({
        ticketCode: ticketCode.trim(),
        deviceId: deviceId.trim(),
        gateId: gateId.trim(),
        source: source.trim() || "ops-center",
        // backend accepts scannedAtUtc per spec
        // @ts-expect-error - extra field accepted by backend
        scannedAtUtc: new Date().toISOString(),
      }),
  });

  const scanId = m.data?.scanAttemptId ?? m.data?.id ?? null;

  const riskQ = useQuery({
    queryKey: ["manual-risk", scanId, lang],
    queryFn: () => api.risk(scanId as string, lang),
    enabled: !!scanId,
    retry: 0,
  });

  const onSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (!ticketCode.trim() || !deviceId.trim() || !gateId.trim()) return;
    m.mutate();
  };

  const valid = ticketCode.trim() && deviceId.trim() && gateId.trim();

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold tracking-tight">{t("manual.title")}</h1>
        <p className="text-sm text-muted-foreground">{t("manual.subtitle")}</p>
      </div>

      <form
        onSubmit={onSubmit}
        className="grid grid-cols-1 gap-4 rounded-lg border bg-card p-5 shadow-sm sm:grid-cols-2"
      >
        <Field label={t("common.ticket")} required>
          <Input
            value={ticketCode}
            onChange={(e) => setTicketCode(e.target.value)}
            placeholder="TKT-12345"
          />
        </Field>
        <Field label={t("common.device")} required>
          <Input
            value={deviceId}
            onChange={(e) => setDeviceId(e.target.value)}
            placeholder="device-001"
          />
        </Field>
        <Field label={t("common.gate")} required>
          <Input
            value={gateId}
            onChange={(e) => setGateId(e.target.value)}
            placeholder="gate-A"
          />
        </Field>
        <Field label={`${t("common.source")} (${t("common.optional")})`}>
          <Input
            value={source}
            onChange={(e) => setSource(e.target.value)}
            placeholder="ops-center"
          />
        </Field>

        <div className="sm:col-span-2">
          <Button type="submit" disabled={!valid || m.isPending}>
            {m.isPending ? (
              <Loader2 className="mr-2 h-4 w-4 animate-spin" />
            ) : (
              <ScanLine className="mr-2 h-4 w-4" />
            )}
            {t("manual.submit")}
          </Button>
        </div>
      </form>

      {m.isError && (
        <div className="flex items-start gap-2 rounded-lg border border-destructive/30 bg-destructive/5 p-4 text-sm text-destructive">
          <AlertCircle className="mt-0.5 h-4 w-4 shrink-0" />
          <div>{m.error instanceof Error ? m.error.message : t("common.error")}</div>
        </div>
      )}

      {m.data && (
        <ResultPanel
          result={m.data}
          risk={riskQ.data}
          riskLoading={riskQ.isFetching}
          riskError={riskQ.isError}
        />
      )}
    </div>
  );
}

function Field({
  label,
  required,
  children,
}: {
  label: string;
  required?: boolean;
  children: React.ReactNode;
}) {
  return (
    <div className="space-y-1.5">
      <Label className="text-xs font-medium text-muted-foreground">
        {label}
        {required && <span className="ml-0.5 text-destructive">*</span>}
      </Label>
      {children}
    </div>
  );
}

function ResultPanel({
  result,
  risk,
  riskLoading,
  riskError,
}: {
  result: ValidateResult;
  risk?: RiskAnalysis;
  riskLoading?: boolean;
  riskError?: boolean;
}) {
  const { t } = useI18n();
  const accepted = result.accepted ?? result.decision?.toLowerCase() === "accepted";
  // Merge: risk endpoint takes precedence for AI fields, fall back to validate response
  const merged: RiskAnalysis = {
    riskScore: risk?.riskScore ?? result.riskScore ?? undefined,
    riskLevel: risk?.riskLevel ?? result.riskLevel ?? undefined,
    recommendedAction: risk?.recommendedAction ?? result.recommendedAction,
    explanationSummary: risk?.explanationSummary ?? result.explanationSummary,
    explanationConfidence:
      risk?.explanationConfidence ?? result.explanationConfidence,
    explanationProvider: risk?.explanationProvider ?? result.explanationProvider,
    riskExplanation: risk?.riskExplanation ?? result.riskExplanation,
    riskSignals: risk?.riskSignals ?? result.riskSignals ?? [],
  };
  const signals = Array.isArray(merged.riskSignals) ? merged.riskSignals : [];

  return (
    <div className="space-y-4">
      <div
        className={
          "flex items-start gap-2 rounded-lg border p-4 text-sm " +
          (accepted
            ? "border-emerald-200 bg-emerald-50 text-emerald-800"
            : "border-amber-200 bg-amber-50 text-amber-800")
        }
      >
        {accepted ? (
          <CheckCircle2 className="mt-0.5 h-4 w-4 shrink-0" />
        ) : (
          <AlertCircle className="mt-0.5 h-4 w-4 shrink-0" />
        )}
        <div className="flex-1">
          <div className="font-medium">{result.message ?? t("manual.result")}</div>
        </div>
      </div>

      <div className="rounded-lg border bg-card p-4 shadow-sm">
        <h2 className="mb-3 text-sm font-semibold">{t("manual.result")}</h2>
        <dl className="grid grid-cols-1 gap-x-6 gap-y-3 sm:grid-cols-2">
          <Row label={t("common.decision")}>
            <DecisionBadge decision={result.decision} />
          </Row>
          <Row label={t("common.reason")}>
            <span className="text-sm">{result.reasonCode ?? "—"}</span>
          </Row>
        </dl>
      </div>

      <div className="rounded-lg border bg-card p-4 shadow-sm">
        <div className="mb-3 flex items-center justify-between gap-2">
          <h2 className="text-sm font-semibold">{t("drawer.aiRisk")}</h2>
          {riskLoading && (
            <span className="inline-flex items-center gap-1.5 text-xs text-muted-foreground">
              <Loader2 className="h-3 w-3 animate-spin" />
              {t("common.loading")}
            </span>
          )}
          {riskError && !riskLoading && (
            <span className="inline-flex items-center gap-1.5 rounded-md border border-amber-200 bg-amber-50 px-2 py-0.5 text-xs text-amber-800">
              <AlertCircle className="h-3 w-3" />
              {t("manual.aiUnavailable")}
            </span>
          )}
        </div>
        <dl className="grid grid-cols-1 gap-x-6 gap-y-3 sm:grid-cols-2">
          <Row label={t("drawer.score")}>
            <span className="text-sm font-medium">
              {typeof merged.riskScore === "number" ? merged.riskScore.toFixed(2) : "—"}
            </span>
          </Row>
          <Row label={t("drawer.level")}>
            <RiskBadge level={merged.riskLevel} />
          </Row>
          <Row label={t("drawer.action")}>
            <span className="text-sm">{merged.recommendedAction ?? "—"}</span>
          </Row>
          <Row label={t("drawer.provider")}>
            <ProviderBadge provider={merged.explanationProvider} />
          </Row>
          <Row label={t("drawer.confidence")}>
            <span className="text-sm">
              {typeof merged.explanationConfidence === "number"
                ? `${(merged.explanationConfidence * 100).toFixed(0)}%`
                : "—"}
            </span>
          </Row>
          <Row label={t("drawer.summary")} full>
            <p className="text-sm text-muted-foreground">
              {merged.explanationSummary ?? "—"}
            </p>
          </Row>
          <Row label={t("drawer.explanation")} full>
            <p className="whitespace-pre-wrap text-sm text-muted-foreground">
              {merged.riskExplanation ?? "—"}
            </p>
          </Row>
        </dl>

        {signals.length > 0 && (
          <div className="mt-4">
            <h3 className="mb-2 text-xs font-semibold uppercase tracking-wide text-muted-foreground">
              {t("drawer.signals")}
            </h3>
            <ul className="space-y-2">
              {signals.map((s, i) => {
                if (typeof s === "string") {
                  return (
                    <li
                      key={i}
                      className="rounded border bg-muted/30 px-3 py-2 text-sm"
                    >
                      {s}
                    </li>
                  );
                }
                return (
                  <li
                    key={i}
                    className="flex items-start justify-between gap-3 rounded border bg-muted/30 px-3 py-2 text-sm"
                  >
                    <div>
                      <div className="font-medium">{s.name ?? "—"}</div>
                      {s.description && (
                        <div className="text-xs text-muted-foreground">{s.description}</div>
                      )}
                    </div>
                    {typeof s.weight === "number" && (
                      <span className="shrink-0 rounded-full bg-background px-2 py-0.5 text-xs font-medium ring-1 ring-inset ring-border">
                        {s.weight.toFixed(2)}
                      </span>
                    )}
                  </li>
                );
              })}
            </ul>
          </div>
        )}
      </div>
    </div>
  );
}

function Row({
  label,
  children,
  full,
}: {
  label: string;
  children: React.ReactNode;
  full?: boolean;
}) {
  return (
    <div className={"flex flex-col " + (full ? "sm:col-span-2" : "")}>
      <dt className="text-xs uppercase tracking-wide text-muted-foreground">{label}</dt>
      <dd className="mt-1">{children}</dd>
    </div>
  );
}
