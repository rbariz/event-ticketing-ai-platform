import type { Scan } from "@/lib/api";
import { DecisionBadge } from "@/components/badges";
import { useI18n } from "@/lib/i18n";

interface Props {
  scans: Scan[];
  onSelect: (id: string) => void;
}

export function ScansTable({ scans, onSelect }: Props) {
  const { t } = useI18n();
  return (
    <div className="overflow-x-auto">
      <table className="w-full text-sm">
        <thead>
          <tr className="border-b bg-muted/40 text-left text-xs uppercase tracking-wide text-muted-foreground">
            <th className="px-4 py-2.5 font-medium">{t("common.time")}</th>
            <th className="px-4 py-2.5 font-medium">{t("common.ticket")}</th>
            <th className="px-4 py-2.5 font-medium">{t("common.decision")}</th>
            <th className="px-4 py-2.5 font-medium">{t("common.reason")}</th>
            <th className="px-4 py-2.5 font-medium">{t("common.gate")}</th>
            <th className="px-4 py-2.5 font-medium">{t("common.device")}</th>
            <th className="px-4 py-2.5 font-medium">{t("common.source")}</th>
          </tr>
        </thead>
        <tbody>
          {scans.map((s) => (
            <tr
              key={s.id}
              onClick={() => onSelect(s.id)}
              className="cursor-pointer border-b last:border-0 hover:bg-accent/50"
            >
            <td className="px-4 py-2.5 text-muted-foreground">{formatTime(s.scannedAtUtc)}</td>
              <td className="px-4 py-2.5 font-mono text-xs">{s.ticketCode}</td>
              <td className="px-4 py-2.5">
                <DecisionBadge decision={s.decision} />
              </td>
              <td className="px-4 py-2.5 text-muted-foreground">{s.reasonCode ?? "—"}</td>
              <td className="px-4 py-2.5">{s.gateId ?? "—"}</td>
              <td className="px-4 py-2.5 font-mono text-xs text-muted-foreground">{s.deviceId ?? "—"}</td>
              <td className="px-4 py-2.5 text-muted-foreground">{s.source ?? "—"}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

function formatTime(t: string) {
  if (!t) return "—";
  try {
    const d = new Date(t);
    if (isNaN(d.getTime())) return "—";
    return d.toLocaleString();
  } catch {
    return t ?? "—";
  }
}