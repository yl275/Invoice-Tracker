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

export default api;
