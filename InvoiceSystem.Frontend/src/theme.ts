export type Theme = "light" | "dark";

const STORAGE_KEY = "theme";

export function getInitialTheme(): Theme {
  if (typeof window === "undefined") return "light";

  const stored = window.localStorage.getItem(STORAGE_KEY);
  if (stored === "light" || stored === "dark") {
    return stored;
  }

  if (window.matchMedia?.("(prefers-color-scheme: dark)").matches) {
    return "dark";
  }

  return "light";
}

function applyTheme(theme: Theme) {
  if (typeof document === "undefined") return;
  const root = document.documentElement;
  if (theme === "dark") {
    root.classList.add("dark");
  } else {
    root.classList.remove("dark");
  }
}

export function setTheme(theme: Theme) {
  if (typeof window !== "undefined") {
    window.localStorage.setItem(STORAGE_KEY, theme);
  }
  applyTheme(theme);
}

export function initTheme() {
  applyTheme(getInitialTheme());
}

