import { createFileRoute } from "@tanstack/react-router";
import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { api, type ScanFilters } from "@/lib/api";
import { useI18n } from "@/lib/i18n";
import { ScanFiltersBar } from "@/components/ScanFilters";
import { ScansTable } from "@/components/ScansTable";
import { ScanDetailDrawer } from "@/components/ScanDetailDrawer";
import { LoadingBlock, ErrorBlock, EmptyBlock } from "@/components/states";

export const Route = createFileRoute("/history")({
  component: HistoryPage,
});

function HistoryPage() {
  const { t } = useI18n();
  const [filters, setFilters] = useState<ScanFilters>({});
  const [openId, setOpenId] = useState<string | null>(null);

  const q = useQuery({
    queryKey: ["scans", filters],
    queryFn: () => api.scans(filters),
  });

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold tracking-tight">{t("history.title")}</h1>
        <p className="text-sm text-muted-foreground">{t("history.subtitle")}</p>
      </div>

      <ScanFiltersBar value={filters} onChange={setFilters} />

      <section className="rounded-lg border bg-card shadow-sm">
        {q.isLoading && <LoadingBlock />}
        {q.isError && <div className="p-4"><ErrorBlock error={q.error} onRetry={() => q.refetch()} /></div>}
        {q.data && (q.data.length ? (
          <ScansTable scans={q.data} onSelect={setOpenId} />
        ) : <EmptyBlock />)}
      </section>

      <ScanDetailDrawer scanId={openId} open={!!openId} onOpenChange={(o) => !o && setOpenId(null)} />
    </div>
  );
}