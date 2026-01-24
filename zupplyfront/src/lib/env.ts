export const APP_ENV =
  process.env.NEXT_PUBLIC_APP_ENV?.toLowerCase() ?? "development";

export const isProd = APP_ENV === "production";
export const isDev = APP_ENV === "development";
