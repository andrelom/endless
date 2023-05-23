---
author: 'André Moraes'
title: 'Seamless Integration of Sitecore Content Hub With Nextjs Using Reverse Proxy Calls'
description: ''
tags: []
date: 2023-05-23T03:47:58-03:00
draft: false
---

Efficient and secure connections between different platforms are crucial in the ever-evolving web development landscape. One such scenario is integrating Sitecore Content Hub with Next.js applications.

By utilizing Next.js and implementing reverse proxy calls, developers can optimize communication while safeguarding sensitive information. In this blog post, we'll explore the benefits of reverse proxy calls and how they enhance security and API protection by concealing the Content Hub endpoint and API token.

## Benefits of Reverse Proxy Calls

When integrating Next.js with Sitecore Content Hub, reverse proxy calls offer an additional layer of security. By routing requests through a reverse proxy server, the Content Hub endpoint remains shielded from direct exposure. This isolation fortifies protection against attacks and unauthorized access attempts, bolstering overall security.

### Hidden Endpoint and API Token

Reverse proxy calls also ensure the concealment of sensitive information, such as the Content Hub endpoint and API token, from client-side code. Rather than making direct requests to the Content Hub API, all requests are directed to the reverse proxy server. This approach minimizes the risk of exposing the endpoint and API token to potential threats, providing heightened protection for sensitive information.

By leveraging reverse proxy calls, developers can enhance security and effectively hide the endpoint and API token. This approach strengthens the integration between Next.js and Sitecore Content Hub, establishing a more secure foundation for content management and delivery.

## Implementation

To get started, we need to install the "http-proxy-middleware" npm module. Open your terminal and run the following command:

```sh
yarn add http-proxy-middleware
```

### Updating the Client File

Next, we'll update the client file "sitecorecloud.ts" with the following code:

```ts
import { createApolloClient } from 'endless-lib/apollo'

const sitecorecloud = createApolloClient({
  uri: process.env.APOLLO_SITECORECLOUD_URI ?? '/api/proxy/sitecorecloud',
  headers: {
    'X-GQL-Token': process.env.APOLLO_SITECORECLOUD_TOKEN ?? '',
  },
})

export default sitecorecloud
```

As you can observe, when executing this call on the server, we direct it to the original endpoint. However, if we're not on the server, we use the reverse proxy endpoint instead. Additionally, please note that environment variables which don't begin with "NEXT_PUBLIC\_" will not be accessible on the client side.

Make sure to replace "endless-lib/apollo" with the appropriate import for your specific setup.

### Creating the API Endpoint (Reverse Proxy)

Now, let's create the API endpoint with the following code:

```ts
// File: pages/api/proxy/sitecorecloud/[[...sitecorecloud]].api.ts

import type { NextApiRequest, NextApiResponse } from 'next'
import { createProxyMiddleware } from 'http-proxy-middleware'

const proxy = createProxyMiddleware({
  secure: false,
  changeOrigin: true,
  target: process.env.APOLLO_SITECORECLOUD_URI,
  pathRewrite: {
    '/api/proxy/sitecorecloud': '',
  },
}) as (req: NextApiRequest, res: NextApiResponse) => void

export const config = {
  api: {
    bodyParser: false,
    externalResolver: true,
  },
}

export default async function handler(req: NextApiRequest, res: NextApiResponse) {
  const header = 'X-GQL-Token'.toLowerCase()

  // Since the value of the environment variable is not public
  // to the client, this header value will be undefined.
  // Therefore, we set it before sending the request.
  req.headers[header] = process.env.APOLLO_SITECORECLOUD_TOKEN

  proxy(req, res)
}
```

This file sets up a reverse proxy using the http-proxy-middleware module. It forwards requests to the specified target ("process.env.APOLLO_SITECORECLOUD_URI") while rewriting the path to remove the proxy part ("/api/system/proxy/sitecorecloud"). The "X-GQL-Token" header is added to the request using the value from the environment variable "APOLLO_SITECORECLOUD_TOKEN".

Remember to adjust the code to match your specific environment and requirements.

## Conclusion

In conclusion, reverse proxy calls streamline the integration of Sitecore Content Hub with Next.js, enhancing security and simplifying the process.

The HTTP Proxy Middleware simplifies the implementation of reverse proxy functionality in Next.js applications by handling low-level configurations. It acts as an intermediary, allowing direct calls to the original endpoint on the back-end while providing security and abstraction for the client-side application.

By leveraging reverse proxy calls and the capabilities of Next.js and the HTTP Proxy Middleware, developers can establish a seamless integration between Sitecore Content Hub and Next.js, ensuring secure and optimized content management and delivery.
