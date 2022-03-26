import { defineConfig } from 'vitepress';

export default defineConfig({
  base: '/JsTimers',
  lang: 'en-US',
  title: 'JsTimers',
  description: 'Simple timer management for .NET',
  head: [['link', { rel: 'icon', href: '/favicon.png', type: 'image/x-icon' }]],
  themeConfig: {
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
