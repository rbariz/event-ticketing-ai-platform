import { createFileRoute } from "@tanstack/react-router";
import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { api, type IncidentFilters } from "@/lib/api";
import { useI18n } from "@/lib/i18n";
import { LoadingBlock, ErrorBlock, EmptyBlock } from "@/components/states";
import { SeverityBadge, IncidentStatusBadge } from "@/components/badges";
import { useIncidentDrawer } from "@/components/IncidentDrawerProvider";

export const Route = createFileRoute("/incidents")({
  component: IncidentsPage,
});

function fmtDate(d?: string | null) {
  if (!d) return "—";
  const t = new Date(d);
  return isNaN(t.getTime()) ? "—" : t.toLocaleString();
}

function IncidentsPage() {
  const { t } = useI18n();
  const { openIncident } = useIncidentDrawer();
  const [filters, setFilters] = useState<IncidentFilters>({ count: 50 });

  const q = useQuery({
    queryKey: ["incidents", filters],
    queryFn: () => api.incidents(filters),
  });

  const list = q.data ?? [];

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold tracking-tight">{t("incidents.title")}</h1>
        <p className="text-sm text-muted-foreground">{t("incidents.subtitle")}</p>
      </div>

      <section className="rounded-lg border bg-card p-3 shadow-sm">
        <div className="grid grid-cols-1 gap-3 sm:grid-cols-3">
          <FilterSelect
            label={t("incidents.status")}
            value={filters.status ?? ""}
            onChange={(v) => setFilters((f) => ({ ...f, status: v || undefined }))}
            options={[
              { value: "", label: t("common.all") },
              { value: "Open", label: t("incidents.statusOpen") },
              { value: "InProgress", label: t("incidents.statusInProgress") },
              { value: "Resolved", label: t("incidents.statusResolved") },
            ]}
          />
          <FilterSelect
            label={t("incidents.severity")}
            value={filters.severity ?? ""}
            onChange={(v) => setFilters((f) => ({ ...f, severity: v || undefined }))}
            options={[
              { value: "", label: t("common.all") },
              { value: "Low", label: "Low" },
              { value: "Medium", label: "Medium" },
              { value: "High", label: "High" },
              { value: "Critical", label: "Critical" },
            ]}
          />
          <div className="flex flex-col gap-1">
            <label className="text-[11px] font-medium uppercase tracking-wide text-muted-foreground">
              {t("incidents.count")}
            </label>
            <input
              type="number"
              min={1}
              max={500}
              value={filters.count ?? 50}
              onChange={(e) =>
                setFilters((f) => ({ ...f, count: Number(e.target.value) || 50 }))
              }
              className="h-9 rounded-md border bg-background px-3 text-sm"
            />
          </div>
        </div>
      </section>

      <section className="rounded-lg border bg-card shadow-sm">
        {q.isLoading && <LoadingBlock />}
        {q.isError && (
          <div className="p-4">
            <ErrorBlock error={q.error} onRetry={() => q.refetch()} />
          </div>
        )}
        {q.data && list.length === 0 && <EmptyBlock label={t("incidents.empty")} />}
        {q.data && list.length > 0 && (
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead className="bg-muted/40 text-left text-[11px] uppercase tracking-wide text-muted-foreground">
                <tr>
                  <th className="px-4 py-2 font-medium">{t("incidents.created")}</th>
                  <th className="px-4 py-2 font-medium">{t("incidents.severity")}</th>
                  <th className="px-4 py-2 font-medium">{t("incidents.status")}</th>
                  <th className="px-4 py-2 font-medium">{t("incidents.title.col")}</th>
                  <th className="px-4 py-2 font-medium">{t("incidents.assignedTo")}</th>
                  <th className="px-4 py-2 font-medium">{t("incidents.scanRef")}</th>
                </tr>
              </thead>
              <tbody className="divide-y">
                {list.map((inc) => (
                  <tr
                    key={inc.id}
                    onClick={() => openIncident(inc.id)}
                    className="cursor-pointer transition-colors hover:bg-muted/40"
                  >
                    <td className="px-4 py-2 tabular-nums text-xs text-muted-foreground">
                      {fmtDate(inc.createdAtUtc)}
                    </td>
                    <td className="px-4 py-2">
                      <SeverityBadge severity={inc.severity} />
                    </td>
                    <td className="px-4 py-2">
                      <IncidentStatusBadge status={inc.status} />
                    </td>
                    <td className="px-4 py-2">
                      <div className="font-medium">{inc.title ?? "—"}</div>
                    </td>
                    <td className="px-4 py-2 text-xs">{inc.assignedTo ?? "—"}</td>
                    <td className="px-4 py-2 font-mono text-[11px] text-muted-foreground">
                      {inc.scanAttemptId ? inc.scanAttemptId.slice(0, 8) : "—"}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </section>
    </div>
  );
}

function FilterSelect({
  label,
  value,
  onChange,
  options,
}: {
  label: string;
  value: string;
  onChange: (v: string) => void;
  options: Array<{ value: string; label: string }>;
}) {
  return (
    <div className="flex flex-col gap-1">
      <label className="text-[11px] font-medium uppercase tracking-wide text-muted-foreground">
        {label}
      </label>
      <select
        value={value}
        onChange={(e) => onChange(e.target.value)}
        className="h-9 rounded-md border bg-background px-3 text-sm"
      >
        {options.map((o) => (
          <option key={o.value} value={o.value}>
            {o.label}
          </option>
        ))}
      </select>
    </div>
  );
}