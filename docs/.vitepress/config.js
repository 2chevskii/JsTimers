import { defineConfig } from 'vitepress';

export default defineConfig({
  base: '/JsTimers',
  lang: 'en-US',
  title: 'JsTimers',
  description: 'Simple timer management for .NET',
  head: [
    [
      'link',
      { rel: 'icon', href: '/JsTimers/favicon.png', type: 'image/x-icon' }
    ]
  ],
  themeConfig: {
    nav: [
      {
        link: 'https://github.com/2chevskii/JsTimers',
        target: '_blank',
        text: 'GitHub'
      },
      {
        link: 'https://nuget.org/packages/JsTimers',
        target: '_blank',
        text: 'NuGet'
      },
      {
        link: 'https://ci.appveyor.com/project/2chevskii/jstimers/builds/43025385',
        target: '_blank',
        text: 'Latest stable build (1.0.38)'
      }
    ],
    sidebar: [
      {
        text: 'Introduction',
        link: '/',
        children: [
          {
            text: 'Installation',
            link: '/getting-started/installation'
          },
          {
            text: 'Basic usage',
            link: '/getting-started/basic-usage'
          }
        ]
      },
      {
        text: 'Guide',
        children: [
          {
            text: 'Timer types',
            link: '/guide/timer-types'
          },
          {
            text: 'Instantiation',
            link: '/guide/instantiation'
          },
          {
            text: 'Cancellation',
            link: '/guide/cancellation'
          },
          {
            text: 'Ref/UnRef',
            link: '/guide/refs'
          }
        ]
      },
      {
        text: 'Credits',
        link: '/credits'
      }
    ]
  }
});
