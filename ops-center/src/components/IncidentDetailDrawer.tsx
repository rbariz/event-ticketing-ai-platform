import { useState, useEffect } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Sheet, SheetContent, SheetHeader, SheetTitle } from "@/components/ui/sheet";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { api } from "@/lib/api";
import { useI18n } from "@/lib/i18n";
import { LoadingBlock, ErrorBlock } from "@/components/states";
import { SeverityBadge, IncidentStatusBadge } from "@/components/badges";
import { useScanDrawer } from "@/components/ScanDrawerProvider";
import { ExternalLink } from "lucide-react";

interface Props {
  incidentId: string | null;
  open: boolean;
  onOpenChange: (o: boolean) => void;
}

function fmtDate(d?: string | null) {
  if (!d) return "—";
  const t = new Date(d);
  return isNaN(t.getTime()) ? "—" : t.toLocaleString();
}

export function IncidentDetailDrawer({ incidentId, open, onOpenChange }: Props) {
  const { t } = useI18n();
  const qc = useQueryClient();
  const { openScan } = useScanDrawer();

  const incQ = useQuery({
    queryKey: ["incident", incidentId],
    queryFn: () => api.incident(incidentId!),
    enabled: !!incidentId && open,
  });

  const [assignee, setAssignee] = useState("");
  const [note, setNote] = useState("");
  const [opError, setOpError] = useState<string | null>(null);

  useEffect(() => {
    setAssignee(incQ.data?.assignedTo ?? "");
    setNote("");
    setOpError(null);
  }, [incQ.data?.id]);

  const refresh = () => {
    qc.invalidateQueries({ queryKey: ["incident", incidentId] });
    qc.invalidateQueries({ queryKey: ["incidents"] });
  };

  const assignMut = useMutation({
    mutationFn: (val: string) => api.assignIncident(incidentId!, val),
    onSuccess: () => {
      setOpError(null);
      refresh();
    },
    onError: (e) => setOpError(e instanceof Error ? e.message : t("incidents.assignFailed")),
  });

  const resolveMut = useMutation({
    mutationFn: (val: string) => api.resolveIncident(incidentId!, val),
    onSuccess: () => {
      setOpError(null);
      setNote("");
      refresh();
    },
    onError: (e) => setOpError(e instanceof Error ? e.message : t("incidents.resolveFailed")),
  });

  const inc = incQ.data;
  const isResolved = (inc?.status ?? "").toLowerCase() === "resolved";

  return (
    <Sheet open={open} onOpenChange={onOpenChange}>
      <SheetContent className="w-full overflow-y-auto sm:max-w-xl">
        <SheetHeader>
          <SheetTitle>{t("incidents.title")}</SheetTitle>
        </SheetHeader>

        {incQ.isLoading && <LoadingBlock />}
        {incQ.isError && (
          <ErrorBlock error={incQ.error} onRetry={() => incQ.refetch()} />
        )}
        {inc && (
          <div className="mt-4 space-y-4">
            <div className="rounded-lg border bg-card p-4 space-y-3">
              <div className="flex flex-wrap items-center gap-2">
                <SeverityBadge severity={inc.severity} />
                <IncidentStatusBadge status={inc.status} />
              </div>
              <div>
                <div className="text-sm font-semibold">{inc.title ?? "—"}</div>
                {inc.description && (
                  <p className="mt-1 text-sm text-muted-foreground whitespace-pre-wrap">
                    {inc.description}
                  </p>
                )}
              </div>
              <dl className="grid grid-cols-2 gap-x-4 gap-y-2 text-xs">
                <Field label={t("incidents.created")} value={fmtDate(inc.createdAtUtc)} />
                <Field label={t("incidents.assigned")} value={fmtDate(inc.assignedAtUtc)} />
                <Field label={t("incidents.assignedTo")} value={inc.assignedTo ?? "—"} />
                <Field label={t("incidents.resolved")} value={fmtDate(inc.resolvedAtUtc)} />
                <Field
                  label={t("incidents.scanRef")}
                  value={
                    <span className="font-mono text-[11px]">
                      {inc.scanAttemptId ? inc.scanAttemptId.slice(0, 8) : "—"}
                    </span>
                  }
                />
              </dl>
              {inc.scanAttemptId && (
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => {
                    onOpenChange(false);
                    openScan(inc.scanAttemptId);
                  }}
                >
                  <ExternalLink className="mr-2 h-3.5 w-3.5" />
                  {t("incidents.openScan")}
                </Button>
              )}
              {inc.resolutionNote && (
                <div className="rounded-md border bg-muted/30 p-3 text-xs">
                  <div className="mb-1 font-medium text-muted-foreground">
                    {t("incidents.resolutionNote")}
                  </div>
                  <div className="whitespace-pre-wrap">{inc.resolutionNote}</div>
                </div>
              )}
            </div>

            {opError && (
              <div className="rounded-md border border-destructive/30 bg-destructive/5 p-3 text-xs text-destructive">
                {opError}
              </div>
            )}

            {!isResolved && (
              <>
                <div className="rounded-lg border bg-card p-4 space-y-2">
                  <label className="text-xs font-medium text-muted-foreground">
                    {t("incidents.assignedTo")}
                  </label>
                  <div className="flex gap-2">
                    <Input
                      value={assignee}
                      onChange={(e) => setAssignee(e.target.value)}
                      placeholder={t("incidents.assignPlaceholder")}
                    />
                    <Button
                      onClick={() => assignee.trim() && assignMut.mutate(assignee.trim())}
                      disabled={assignMut.isPending || !assignee.trim()}
                    >
                      {t("incidents.assign")}
                    </Button>
                  </div>
                </div>

                <div className="rounded-lg border bg-card p-4 space-y-2">
                  <label className="text-xs font-medium text-muted-foreground">
                    {t("incidents.resolutionNote")}
                  </label>
                  <Textarea
                    value={note}
                    onChange={(e) => setNote(e.target.value)}
                    placeholder={t("incidents.notePlaceholder")}
                    rows={3}
                  />
                  <div className="flex justify-end">
                    <Button
                      onClick={() => note.trim() && resolveMut.mutate(note.trim())}
                      disabled={resolveMut.isPending || !note.trim()}
                    >
                      {t("incidents.resolve")}
                    </Button>
                  </div>
                </div>
              </>
            )}
          </div>
        )}
      </SheetContent>
    </Sheet>
  );
}

function Field({ label, value }: { label: string; value: React.ReactNode }) {
  return (
    <div>
      <dt className="text-[10px] uppercase tracking-wide text-muted-foreground">
        {label}
      </dt>
      <dd className="mt-0.5">{value}</dd>
    </div>
  );
}