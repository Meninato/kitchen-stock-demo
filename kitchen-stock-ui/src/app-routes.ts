export interface NavItem {
  title: string;
  href: string;
}

export interface NavSection {
  title: string;
  href: string;
  icon?: React.ElementType;
  items?: NavItem[];
}

export const APP_ROUTES = {
  AUTH: {
    SIGN_IN: "/auth/sign-in",
    SIGN_UP: "/auth/sign-up",
    FORGOT_PASSWORD: "/auth/forgot-password",
  },
  APP: {
    HOME: "/app",
    KITCHEN_SELECTION: "/app/kitchen-selection",
    INGREDIENTS: "/app/ingredients",
    RECIPES: "/app/recipes",
    SUPPLIERS: "/app/suppliers",
  },
};

export const sidebarNav: NavSection[] = [
  {
    title: "Mesa da casa", //mesa da casa, pronto para cozinhar, o que tem em casa
    href: "#",
    items: [
      {
        title: "Igredientes",
        href: APP_ROUTES.APP.INGREDIENTS,
      },
      {
        title: "Receitas",
        href: "#",
      },
      {
        title: "Fornecedores",
        href: "#",
      },
    ],
  },
];
