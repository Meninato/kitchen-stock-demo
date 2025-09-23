import { cookies } from "next/headers";

const refreshCookieName = "refresh_token";
const accessCookieName = "access_token";

export const serverTokenStorage = {
  async getAccessToken(): Promise<string | null> {
    try {
      const cookieStore = await cookies();
      const token = cookieStore.get(accessCookieName)?.value || null;
      return token;
    } catch {
      return null;
    }
  },

  async getRefreshToken(): Promise<string | null> {
    try {
      const cookieStore = await cookies();
      const token = cookieStore.get(refreshCookieName)?.value || null;
      return token;
    } catch {
      return null;
    }
  },

  async destroyTokens(): Promise<void> {
    const cookieStore = await cookies();
    cookieStore.delete(refreshCookieName);
    cookieStore.delete(accessCookieName);
  },

  async hasAccessToken(): Promise<boolean> {
    const token = await this.getAccessToken();
    return !!token;
  },

  async hasRefreshToken(): Promise<boolean> {
    const token = await this.getRefreshToken();
    return !!token;
  },
};
