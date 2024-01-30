import { defineConfig } from "vitepress";

export default defineConfig({
  title: "JsTimers",
  description: "JsTimers documentation",
  base: "/JsTimers/",
  head: [
    [
      "link",
      {
        rel: "icon",
        href: "/JsTimers/favicon.ico",
      },
    ],
  ],

  markdown: {
    theme: "aurora-x",
  },

  appearance: "force-dark",

  themeConfig: {
    nav: [
      { text: "Home", link: "/" },
      { text: "Guide", link: "/guide/installation" },
      { text: "API", link: "/api/" },
    ],

    sidebar: [
      {
        text: "Guide",
        items: [
          {
            text: "Installation",
            link: "/guide/installation",
          },
          {
            text: "Basic usage",
            link: "/guide/basic-usage",
          },
          {
            text: "Advanced topics",
            items: [
              {
                text: "Timer types",
                link: "/guide/advanced/timer-types",
              },
              {
                text: "Instantiation",
                link: "/guide/advanced/instantiation",
              },
              {
                text: "Cancellation",
                link: "/guide/advanced/cancellation",
              },
              {
                text: "Refs",
                link: "/guide/advanced/refs",
              },
            ],
          },
        ],
      },
      {
        text: "API",
        link: "/api/",
        items: [
          {
            text: "JsTimers",
            items: [
              {
                text: "TimerManager",
                link: "/api/JsTimers/TimerManager",
              },
              {
                text: "Timer",
                link: "/api/JsTimers/Timer",
              },
              {
                text: "Timeout",
                link: "/api/JsTimers/Timeout",
              },
              {
                text: "Immediate",
                link: "/api/JsTimers/Immediate",
              },
            ],
          },
        ],
      },
    ],

    socialLinks: [
      { icon: "github", link: "https://github.com/2chevskii/JsTimers" },
    ],
    search: { provider: "local" },
  },
});
