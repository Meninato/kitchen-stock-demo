import { cookies } from "next/headers";

export const serverTokenStorage = {
  async getAccessToken(): Promise<string | null> {
    try {
      const cookieStore = await cookies();
      const token = cookieStore.get("access_token")?.value || null;
      return token;
    } catch {
      return null;
    }
  },

  async getRefreshToken(): Promise<string | null> {
    try {
      const cookieStore = await cookies();
      const token = cookieStore.get("refresh_token")?.value || null;
      return token;
    } catch {
      return null;
    }
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
