import { createFileRoute } from "@tanstack/react-router";
import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { Search } from "lucide-react";
import { api, ApiError, type Scan, type Ticket } from "@/lib/api";
import { useI18n } from "@/lib/i18n";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { DecisionBadge } from "@/components/badges";
import { LoadingBlock, ErrorBlock, EmptyBlock } from "@/components/states";
import { ScanDetailDrawer } from "@/components/ScanDetailDrawer";

export const Route = createFileRoute("/ticket-lookup")({
  component: TicketLookupPage,
});

function fmtDate(v?: string | null) {
  if (!v) return "—";
  const d = new Date(v);
  if (isNaN(d.getTime())) return "—";
  return d.toLocaleString();
}

function TicketLookupPage() {
  const { t } = useI18n();
  const [code, setCode] = useState("");
  const [submitted, setSubmitted] = useState<string | null>(null);
  const [activeScanId, setActiveScanId] = useState<string | null>(null);
  const [drawerOpen, setDrawerOpen] = useState(false);

  const ticketQ = useQuery<Ticket | null>({
    queryKey: ["ticket", submitted],
    queryFn: async () => {
      if (!submitted) return null;
      try {
        return await api.ticketByCode(submitted);
      } catch (e) {
        if (e instanceof ApiError && e.status === 404) return null;
        throw e;
      }
    },
    enabled: !!submitted,
  });

  const scansQ = useQuery<Scan[]>({
    queryKey: ["scans-all-for-ticket", submitted],
    queryFn: () => api.scans({}),
    enabled: !!submitted,
  });

  const relatedScans = (scansQ.data ?? []).filter(
    (s) => submitted && s.ticketCode?.toLowerCase() === submitted.toLowerCase(),
  );

  const onSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    const v = code.trim();
    if (v) setSubmitted(v);
  };

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold tracking-tight">{t("lookup.title")}</h1>
        <p className="text-sm text-muted-foreground">{t("lookup.subtitle")}</p>
      </div>

      <form
        onSubmit={onSubmit}
        className="flex flex-col gap-2 rounded-lg border bg-card p-4 shadow-sm sm:flex-row"
      >
        <Input
          value={code}
          onChange={(e) => setCode(e.target.value)}
          placeholder={t("lookup.placeholder")}
          className="flex-1"
        />
        <Button type="submit" disabled={!code.trim() || ticketQ.isFetching}>
          <Search className="mr-2 h-4 w-4" />
          {t("common.search")}
        </Button>
      </form>

      {!submitted && <EmptyBlock />}

      {submitted && (
        <section className="rounded-lg border bg-card shadow-sm">
          {ticketQ.isLoading && <LoadingBlock />}
          {ticketQ.isError && (
            <div className="p-4">
              <ErrorBlock error={ticketQ.error} onRetry={() => ticketQ.refetch()} />
            </div>
          )}
          {!ticketQ.isLoading && !ticketQ.isError && !ticketQ.data && (
            <div className="p-6 text-center text-sm text-muted-foreground">
              {t("lookup.notFound")}
            </div>
          )}
          {ticketQ.data && (
            <TicketCard ticket={ticketQ.data} searchedCode={submitted} />
          )}
        </section>
      )}

      {submitted && (
        <section className="rounded-lg border bg-card shadow-sm">
          <div className="border-b px-4 py-3">
            <h2 className="text-sm font-semibold">{t("lookup.related")}</h2>
          </div>
          {scansQ.isLoading && <LoadingBlock />}
          {scansQ.isError && (
            <div className="p-4">
              <ErrorBlock error={scansQ.error} onRetry={() => scansQ.refetch()} />
            </div>
          )}
          {scansQ.data && (relatedScans.length ? (
            <RelatedScansTable
              scans={relatedScans}
              onSelect={(id) => {
                setActiveScanId(id);
                setDrawerOpen(true);
              }}
            />
          ) : (
            <EmptyBlock />
          ))}
        </section>
      )}

      <ScanDetailDrawer
        scanId={activeScanId}
        open={drawerOpen}
        onOpenChange={(o) => {
          setDrawerOpen(o);
          if (!o) setActiveScanId(null);
        }}
      />
    </div>
  );
}

function TicketCard({ ticket, searchedCode }: { ticket: Ticket; searchedCode: string }) {
  const rows: Array<[string, string]> = [
    [
      "Ticket code",
      ((ticket as any).ticketCode as string | undefined) ??
        ticket.code ??
        searchedCode ??
        "—",
    ],
    ["Status", (ticket.status as string) ?? "—"],
    ["Valid from", fmtDate(ticket.validFromUtc as string | undefined)],
    ["Valid until", fmtDate(ticket.validUntilUtc as string | undefined)],
    ["Consumed at", fmtDate(ticket.consumedAtUtc as string | undefined)],
  ];
  return (
    <dl className="grid grid-cols-1 gap-x-6 gap-y-3 p-4 sm:grid-cols-2">
      {rows.map(([k, v]) => (
        <div key={k} className="flex flex-col">
          <dt className="text-xs uppercase tracking-wide text-muted-foreground">{k}</dt>
          <dd className="text-sm font-medium">{v}</dd>
        </div>
      ))}
    </dl>
  );
}

function RelatedScansTable({
  scans,
  onSelect,
}: {
  scans: Scan[];
  onSelect: (id: string) => void;
}) {
  return (
    <div className="overflow-x-auto">
      <table className="w-full text-sm">
        <thead>
          <tr className="border-b bg-muted/40 text-left text-xs uppercase tracking-wide text-muted-foreground">
            <th className="px-4 py-2.5 font-medium">Time</th>
            <th className="px-4 py-2.5 font-medium">Decision</th>
            <th className="px-4 py-2.5 font-medium">Reason</th>
            <th className="px-4 py-2.5 font-medium">Gate</th>
            <th className="px-4 py-2.5 font-medium">Device</th>
          </tr>
        </thead>
        <tbody>
          {scans.map((s) => (
            <tr
              key={s.id}
              onClick={() => onSelect(s.id)}
              className="cursor-pointer border-b transition-colors last:border-0 hover:bg-muted/40"
            >
              <td className="px-4 py-2.5 text-muted-foreground">{fmtDate(s.scannedAtUtc)}</td>
              <td className="px-4 py-2.5">
                <DecisionBadge decision={s.decision} />
              </td>
              <td className="px-4 py-2.5 text-muted-foreground">{s.reasonCode ?? "—"}</td>
              <td className="px-4 py-2.5">{s.gateId ?? "—"}</td>
              <td className="px-4 py-2.5 font-mono text-xs text-muted-foreground">
                {s.deviceId ?? "—"}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
