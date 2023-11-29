---
author: 'André Moraes'
title: 'Navigating the Personalization Landscape: A Case for Next.js Static Site Generation with Sitecore XM Cloud'
description: 'Explore the power of Next.js Static Site Generation in a integration with Sitecore XM Cloud. Uncover the benefits of personalized user experiences while overcoming the challenges of server-side rendering.'
tags: ['Sitecore', 'Sitecore XM Cloud', 'Personalization', 'Static Site Generation', 'Next.js']
date: 2023-11-24T04:25:32-03:00
draft: false
---

In a recent webinar, I dived deep into the seamless integration of Sitecore XM Cloud and Next.js Static Site Generation (SSG) to not only unlock efficient and personalized user experiences but also mitigate the downsides associated with server-side rendering. This blog post distills the key insights from the webinar, shedding light on the benefits of SSG and the pitfalls of relying solely on server-side rendering, especially in scenarios involving Sitecore XM Cloud's Connected and Disconnected modes.

### The Pitfalls of Server-Side Rendering

In many applications dealing with Sitecore XM Cloud, the need for Connected and Disconnected modes is a common reality. In Disconnected mode, numerous GraphQL calls to each component, along with integrations with various vendors, may result in a significant overhead. The traditional server-side rendering approach necessitates repeating all these requests on every page request, leading to resource-intensive operations and potentially hindering overall performance.

### The Power of Internal URL Rewriting

To address these challenges, the webinar emphasized the approach of internal URL rewriting within the Next.js middleware. By leveraging this technique, teams can achieve Static Site Generation (SSG) and still deliver unique and personalized output for each user, without the resource-intensive nature of server-side rendering.

### File System Structure Evolution

A significant structural change in the file system route enhances this approach. The root file, previously located at "src\pages\\[[...path]].tsx," has been strategically relocated to "\src\pages\\[country]\\[username]\\[[...path]].tsx." This shift optimizes the organization of dynamic content and supports the personalization goals of individual user sessions.

#### Example Code:

```typescript
import type { NextRequest, NextFetchEvent } from 'next/server'

import { NextResponse } from 'next/server'
import middleware from 'lib/middleware'

const isHTMLDocument = (req: NextRequest): boolean => {
  return req.headers.get('accept')?.toLowerCase().includes('text/html') ?? false
}

export const rewrite = (req: NextRequest) => {
  const url = req.nextUrl.clone()
  const cookie = req.cookies.get('next.session.user')
  const country = req.geo?.country?.toLowerCase() ?? 'global'
  const username = cookie?.value ?? 'anonymous'

  let pathname = ''

  pathname += `/${country}`
  pathname += `/${username}`
  pathname += `/${url.pathname}`

  url.pathname = pathname.replace(/\/+/gi, '/')

  // Enable per-session caching for "getStaticProps".
  return NextResponse.rewrite(url)
}

export const config = {
  matcher: ['/', '/((?!api/|_next/|sitecore/api/|-/|healthz).*)'],
}

export default async function handler(req: NextRequest, event: NextFetchEvent) {
  const res = await middleware(req, event)

  if (isHTMLDocument(req)) {
    return rewrite(req)
  }

  return res
}
```

### The Advantages of Next.js SSG:

1. **Resource-Efficiency:** By transitioning to Next.js SSG, teams can significantly reduce resource consumption, especially in scenarios involving Connected and Disconnected modes in Sitecore XM Cloud.

2. **Personalization without Overhead:** Achieve personalized output for each user session without the need for repeated GraphQL calls and integrations on every page request, optimizing both performance and user experience.

### Closing Thoughts

While personalization is a crucial aspect of modern web applications, the webinar emphasized the need to carefully consider the downsides of server-side rendering, especially in resource-intensive scenarios. The use of internal URL rewriting within the Next.js middleware emerges as a robust solution, enabling Static Site Generation and personalized content delivery without compromising performance.

For a more in-depth example to explore the implementation, please refer to the source code available in [this Github repository](https://github.com/andrelom/xmcloud-demo).
