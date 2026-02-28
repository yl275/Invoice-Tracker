import axios from "axios";

// Production (e.g. Render): VITE_API_HOST from env, then baseURL = https://<host>/api
// Development: use /api and let Vite proxy to the local API
const baseURL =
  import.meta.env.VITE_API_HOST != null && import.meta.env.VITE_API_HOST !== ""
    ? `https://${import.meta.env.VITE_API_HOST}/api`
    : "/api";

const api = axios.create({
  baseURL,
  headers: {
    "Content-Type": "application/json",
  },
});

let authInterceptorId: number | null = null;

/** Set auth: pass getToken when using Clerk, or null for dev bypass (X-User-Id). */
export function setApiAuth(getToken: (() => Promise<string | null>) | null) {
  if (authInterceptorId != null) {
    api.interceptors.request.eject(authInterceptorId);
  }
  authInterceptorId = api.interceptors.request.use(async (config) => {
    const token = getToken ? await getToken() : null;
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    } else if (import.meta.env.DEV) {
      config.headers["X-User-Id"] = "user_demo";
    }
    return config;
  });
}

export default api;
