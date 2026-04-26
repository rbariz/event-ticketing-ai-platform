import { createContext, useContext, useEffect, useState, type ReactNode } from "react";

export type Lang = "en" | "fr";

const STORAGE_KEY = "ops.lang";

const dict = {
  en: {
    "app.title": "TicketGuard Ops",
    "nav.dashboard": "Dashboard",
    "nav.history": "Scan History",
    "nav.lookup": "Ticket Lookup",
    "nav.manual": "Manual Scan",
    "common.language": "Language",
    "common.loading": "Loading…",
    "common.error": "Something went wrong.",
    "common.retry": "Retry",
    "common.empty": "No data to display",
    "common.search": "Search",
    "common.apply": "Apply",
    "common.reset": "Reset",
    "common.close": "Close",
    "common.from": "From (UTC)",
    "common.to": "To (UTC)",
    "common.gate": "Gate",
    "common.source": "Source",
    "common.decision": "Decision",
    "common.reason": "Reason code",
    "common.all": "All",
    "common.any": "Any",
    "common.time": "Time",
    "common.ticket": "Ticket code",
    "common.device": "Device",
    "common.actions": "Actions",
    "common.view": "View",
    "common.optional": "optional",
    "kpi.total": "Total scans",
    "kpi.accepted": "Accepted",
    "kpi.rejected": "Rejected",
    "kpi.duplicate": "Duplicate",
    "kpi.expired": "Expired",
    "kpi.alreadyUsed": "Already used",
    "kpi.highRisk": "High risk",
    "dashboard.title": "Operations dashboard",
    "dashboard.subtitle": "Real-time anti-fraud overview",
    "dashboard.topReasons": "Top reject reasons",
    "dashboard.topGates": "Top gates",
    "dashboard.recent": "Recent scans",
    "history.title": "Scan history",
    "history.subtitle": "Browse and filter ticket scans",
    "drawer.title": "Scan detail",
    "drawer.aiRisk": "AI risk analysis",
    "drawer.score": "Risk score",
    "drawer.level": "Risk level",
    "drawer.action": "Recommended action",
    "drawer.summary": "Summary",
    "drawer.confidence": "Confidence",
    "drawer.provider": "Provider",
    "drawer.explanation": "Explanation",
    "drawer.signals": "Risk signals",
    "lookup.title": "Ticket lookup",
    "lookup.subtitle": "Find a ticket by its code",
    "lookup.placeholder": "Enter ticket code",
    "lookup.notFound": "No ticket found for that code",
    "lookup.related": "Related scan attempts",
    "manual.title": "Manual scan",
    "manual.subtitle": "Run a validation against the antifraud engine",
    "manual.submit": "Validate",
    "manual.result": "Validation result",
    "manual.aiUnavailable": "AI analysis unavailable",
  },
  fr: {
    "app.title": "TicketGuard Ops",
    "nav.dashboard": "Tableau de bord",
    "nav.history": "Historique des scans",
    "nav.lookup": "Recherche de billet",
    "nav.manual": "Scan manuel",
    "common.language": "Langue",
    "common.loading": "Chargement…",
    "common.error": "Une erreur est survenue.",
    "common.retry": "Réessayer",
    "common.empty": "Aucune donnée à afficher",
    "common.search": "Rechercher",
    "common.apply": "Appliquer",
    "common.reset": "Réinitialiser",
    "common.close": "Fermer",
    "common.from": "Du (UTC)",
    "common.to": "Au (UTC)",
    "common.gate": "Porte",
    "common.source": "Source",
    "common.decision": "Décision",
    "common.reason": "Code motif",
    "common.all": "Tous",
    "common.any": "Tout",
    "common.time": "Heure",
    "common.ticket": "Code billet",
    "common.device": "Appareil",
    "common.actions": "Actions",
    "common.view": "Voir",
    "common.optional": "optionnel",
    "kpi.total": "Scans totaux",
    "kpi.accepted": "Acceptés",
    "kpi.rejected": "Rejetés",
    "kpi.duplicate": "Doublons",
    "kpi.expired": "Expirés",
    "kpi.alreadyUsed": "Déjà utilisés",
    "kpi.highRisk": "Risque élevé",
    "dashboard.title": "Centre d'opérations",
    "dashboard.subtitle": "Vue antifraude en temps réel",
    "dashboard.topReasons": "Motifs de rejet principaux",
    "dashboard.topGates": "Portes principales",
    "dashboard.recent": "Scans récents",
    "history.title": "Historique des scans",
    "history.subtitle": "Parcourir et filtrer les scans",
    "drawer.title": "Détail du scan",
    "drawer.aiRisk": "Analyse de risque IA",
    "drawer.score": "Score de risque",
    "drawer.level": "Niveau de risque",
    "drawer.action": "Action recommandée",
    "drawer.summary": "Résumé",
    "drawer.confidence": "Confiance",
    "drawer.provider": "Fournisseur",
    "drawer.explanation": "Explication",
    "drawer.signals": "Signaux de risque",
    "lookup.title": "Recherche de billet",
    "lookup.subtitle": "Trouver un billet par son code",
    "lookup.placeholder": "Saisir le code du billet",
    "lookup.notFound": "Aucun billet trouvé pour ce code",
    "lookup.related": "Tentatives de scan associées",
    "manual.title": "Scan manuel",
    "manual.subtitle": "Lancer une validation antifraude",
    "manual.submit": "Valider",
    "manual.result": "Résultat de la validation",
    "manual.aiUnavailable": "Analyse IA indisponible",
  },
} as const;

export type TKey = keyof typeof dict.en;

interface I18nCtx {
  lang: Lang;
  setLang: (l: Lang) => void;
  t: (k: TKey) => string;
}

const Ctx = createContext<I18nCtx | null>(null);

export function I18nProvider({ children }: { children: ReactNode }) {
  const [lang, setLangState] = useState<Lang>("en");

  useEffect(() => {
    try {
      const stored = localStorage.getItem(STORAGE_KEY);
      if (stored === "en" || stored === "fr") setLangState(stored);
    } catch {}
  }, []);

  const setLang = (l: Lang) => {
    setLangState(l);
    try {
      localStorage.setItem(STORAGE_KEY, l);
    } catch {}
  };

  const t = (k: TKey) => dict[lang][k] ?? dict.en[k] ?? k;

  return <Ctx.Provider value={{ lang, setLang, t }}>{children}</Ctx.Provider>;
}

export function useI18n() {
  const ctx = useContext(Ctx);
  if (!ctx) throw new Error("useI18n must be used within I18nProvider");
  return ctx;
}