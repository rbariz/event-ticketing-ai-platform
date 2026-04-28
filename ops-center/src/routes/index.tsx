import { createFileRoute } from "@tanstack/react-router";
import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { api, type ScanFilters } from "@/lib/api";
import { useI18n } from "@/lib/i18n";
import { ScanFiltersBar } from "@/components/ScanFilters";
import { ScansTable } from "@/components/ScansTable";
import { LoadingBlock, ErrorBlock, EmptyBlock } from "@/components/states";
import { AgentDecisionList } from "@/components/AgentDecisionList";
import { useScanDrawer } from "@/components/ScanDrawerProvider";
import { useIncidentDrawer } from "@/components/IncidentDrawerProvider";
import { SeverityBadge } from "@/components/badges";
import {
  CheckCircle2, XCircle, Copy, Clock, RotateCcw, ShieldAlert, Activity,
} from "lucide-react";

export const Route = createFileRoute("/")({
  component: DashboardPage,
});

function DashboardPage() {
  const { t } = useI18n();
  const [filters, setFilters] = useState<ScanFilters>({});
  const { openScan } = useScanDrawer();
  const { openIncident } = useIncidentDrawer();

  const summaryQ = useQuery({
    queryKey: ["dashboard", "summary", filters],
    queryFn: () => api.dashboardSummary(filters),
  });

  const recentQ = useQuery({
    queryKey: ["scans", "recent", 20],
    queryFn: () => api.recentScans(20),
  });

  const decisionsQ = useQuery({
    queryKey: ["agent", "decision-logs", 5],
    queryFn: () => api.agentDecisionLogs(5),
  });

  const openIncidentsQ = useQuery({
    queryKey: ["incidents", { status: "Open", count: 5 }],
    queryFn: () => api.incidents({ status: "Open", count: 5 }),
  });

  const decisionLogs50Q = useQuery({
    queryKey: ["agent", "decision-logs", 50],
    queryFn: () => api.agentDecisionLogs(50),
  });

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold tracking-tight">{t("dashboard.title")}</h1>
        <p className="text-sm text-muted-foreground">{t("dashboard.subtitle")}</p>
      </div>

      <ScanFiltersBar value={filters} onChange={setFilters} />

      {summaryQ.isLoading && <LoadingBlock />}
      {summaryQ.isError && <ErrorBlock error={summaryQ.error} onRetry={() => summaryQ.refetch()} />}
      {summaryQ.data && (
        <>
          <div className="grid grid-cols-2 gap-3 sm:grid-cols-3 lg:grid-cols-4 xl:grid-cols-7">
            <Kpi icon={Activity} label={t("kpi.total")} value={summaryQ.data.totalScans} tone="primary" />
            <Kpi icon={CheckCircle2} label={t("kpi.accepted")} value={summaryQ.data.acceptedScans} tone="success" />
            <Kpi icon={XCircle} label={t("kpi.rejected")} value={summaryQ.data.rejectedScans} tone="danger" />
            <Kpi icon={Copy} label={t("kpi.duplicate")} value={summaryQ.data.duplicateScans} tone="warning" />
            <Kpi icon={Clock} label={t("kpi.expired")} value={summaryQ.data.expiredScans} tone="muted" />
            <Kpi icon={RotateCcw} label={t("kpi.alreadyUsed")} value={summaryQ.data.alreadyUsedScans} tone="warning" />
            <Kpi icon={ShieldAlert} label={t("kpi.highRisk")} value={summaryQ.data.highRiskScans} tone="danger" />
          </div>

          <div className="grid grid-cols-1 gap-4 lg:grid-cols-2">
            <Card title={t("dashboard.topReasons")}>
              {(summaryQ.data.topRejectReasons ?? []).length ? (
                <BarList items={(summaryQ.data.topRejectReasons ?? []).map((r) => ({
                  label: r.reasonCode, value: r.count,
                }))} />
              ) : <EmptyBlock />}
            </Card>
            <Card title={t("dashboard.topGates")}>
              {(summaryQ.data.topGates ?? []).length ? (
                <BarList items={(summaryQ.data.topGates ?? []).map((g) => ({
                  label: g.gateId, value: g.count,
                }))} />
              ) : <EmptyBlock />}
            </Card>
          </div>
        </>
      )}

      <Card title={t("dashboard.recent")}>
        {recentQ.isLoading && <LoadingBlock />}
        {recentQ.isError && <ErrorBlock error={recentQ.error} onRetry={() => recentQ.refetch()} />}
        {recentQ.data && ((recentQ.data ?? []).length ? (
          <ScansTable scans={recentQ.data ?? []} onSelect={openScan} />
        ) : <EmptyBlock />)}
      </Card>

      <Card title={t("agent.recent")}>
        {decisionsQ.isLoading && <LoadingBlock />}
        {decisionsQ.isError && (
          <div className="p-4">
            <ErrorBlock error={decisionsQ.error} onRetry={() => decisionsQ.refetch()} />
          </div>
        )}
        {decisionsQ.data &&
          ((decisionsQ.data ?? []).length ? (
            <AgentDecisionList logs={decisionsQ.data ?? []} compact />
          ) : (
            <EmptyBlock />
          ))}
      </Card>

      <Card title={t("incidents.open")}>
        {openIncidentsQ.isLoading && <LoadingBlock />}
        {openIncidentsQ.isError && (
          <div className="p-4">
            <ErrorBlock error={openIncidentsQ.error} onRetry={() => openIncidentsQ.refetch()} />
          </div>
        )}
        {openIncidentsQ.data &&
          ((openIncidentsQ.data ?? []).length ? (
            <ul className="divide-y">
              {(openIncidentsQ.data ?? []).map((inc) => {
                const log = (decisionLogs50Q.data ?? []).find(
                  (l) => l.scanAttemptId === inc.scanAttemptId,
                );
                const isFallback =
                  (log?.enrichmentProvider ?? "").toLowerCase() === "rulebased";
                const isEnriched = !!log?.operatorSummary;
                return (
                <li
                  key={inc.id}
                  onClick={() => openIncident(inc.id)}
                  className="flex cursor-pointer items-center gap-3 px-4 py-3 transition-colors hover:bg-muted/40"
                >
                  <SeverityBadge severity={inc.severity} />
                  <div className="min-w-0 flex-1">
                    <div className="truncate text-sm font-medium">{inc.title ?? "—"}</div>
                  </div>
                  {isEnriched && (
                    <span className="rounded-full bg-emerald-50 px-2 py-0.5 text-[10px] font-medium text-emerald-700 ring-1 ring-inset ring-emerald-600/20">
                      {t("incidents.enriched")}
                    </span>
                  )}
                  {!isEnriched && isFallback && (
                    <span className="rounded-full bg-amber-50 px-2 py-0.5 text-[10px] font-medium text-amber-700 ring-1 ring-inset ring-amber-600/20">
                      {t("incidents.fallback")}
                    </span>
                  )}
                  <span className="text-xs tabular-nums text-muted-foreground">
                    {inc.createdAtUtc ? new Date(inc.createdAtUtc).toLocaleString() : "—"}
                  </span>
                </li>
                );
              })}
            </ul>
          ) : (
            <EmptyBlock label={t("incidents.empty")} />
          ))}
      </Card>
    </div>
  );
}

function Card({ title, children }: { title: string; children: React.ReactNode }) {
  return (
    <section className="rounded-lg border bg-card shadow-sm">
      <div className="border-b px-4 py-3">
        <h2 className="text-sm font-semibold">{title}</h2>
      </div>
      <div>{children}</div>
    </section>
  );
}

function Kpi({
  icon: Icon,
  label,
  value,
  tone,
}: {
  icon: React.ComponentType<{ className?: string }>;
  label: string;
  value: number;
  tone: "primary" | "success" | "danger" | "warning" | "muted";
}) {
  const toneClass = {
    primary: "bg-primary/10 text-primary",
    success: "bg-emerald-50 text-emerald-700",
    danger: "bg-red-50 text-red-700",
    warning: "bg-amber-50 text-amber-700",
    muted: "bg-muted text-muted-foreground",
  }[tone];
  return (
    <div className="rounded-lg border bg-card p-4 shadow-sm">
      <div className={`mb-3 inline-flex h-8 w-8 items-center justify-center rounded-md ${toneClass}`}>
        <Icon className="h-4 w-4" />
      </div>
      <div className="text-[11px] uppercase tracking-wide text-muted-foreground">{label}</div>
      <div className="mt-1 text-2xl font-bold tabular-nums">{(value ?? 0).toLocaleString()}</div>
    </div>
  );
}

function BarList({ items }: { items: Array<{ label: string; value: number }> }) {
  const max = Math.max(...items.map((i) => i.value ?? 0), 1);
  return (
    <ul className="space-y-2 p-4">
      {items.slice(0, 8).map((it) => (
        <li key={it.label} className="space-y-1">
          <div className="flex items-center justify-between text-xs">
            <span className="font-medium">{it.label}</span>
            <span className="tabular-nums text-muted-foreground">{(it.value ?? 0).toLocaleString()}</span>
          </div>
          <div className="h-1.5 w-full overflow-hidden rounded-full bg-muted">
            <div
              className="h-full bg-primary"
              style={{ width: `${((it.value ?? 0) / max) * 100}%` }}
            />
          </div>
        </li>
      ))}
    </ul>
  );
}
