---
author: 'André Moraes'
title: 'Get site context item in JSS API and GraphQL calls'
description: 'How to get corresponding site context item in JSS API and GraphQL calls.'
tags: []
date: 2023-01-28T21:16:50-03:00
draft: false
---

In some projects we are faced with activities that lead us to create new pipelines or even a new service layout for applications using Sitecore JSS. In these situations, it is common to need to access the site's context item.

The problem is that many times these pipelines or layout services are used either for API calls (a.k.a. "/sitecore/api/layout/render/jss") or GraphQL (a.k.a. "/sitecore/api/graph/edge"). In the execution context of both situations, access to the context item alternates, and in API-type calls, the object will not point to the site item we expect.

Endpoints:

- JSS API: /sitecore/api/layout/render/jss?sc_apikey={GUID}&item={GUID}
- JSS GraphQL: /sitecore/api/graph/edge?sc_apikey={GUID}

To solve this problem, we can create a helper that will act whether the call is made through the JSS or GraphQL API, and will return the site's relative context item for both situations.

## Get to Work

First, we need to ensure that we will have access to the site item from any content item, that way, regardless of which item we get from the context, we will know which site it belongs to.

```C#
public static Item GetRelativeSite(this Item item)
{
    var site = SiteContextFactory.Sites
        .Where(entry => !string.IsNullOrWhiteSpace(entry.RootPath) && item.Paths.Path.StartsWith(entry.RootPath, StringComparison.OrdinalIgnoreCase))
        .OrderByDescending(entry => entry.RootPath.Length)
        .FirstOrDefault();

    if (site == null)
    {
        return null;
    }

    return Utilities.GetItem(site.RootPath);
}
```

With this method available above (which, by the way, can be useful in several other situations), we can create the solution that will help us access the site item.

Now we can do a very simple operation. When JSS calls are made via the standard API, the home item ID is sent via the URL Query, as the one we usually use ("Sitecore.Context.Item") will not point to the site item we expect.

If this value is available in the HTTP Context URL, we can access the home item, otherwise (the "item" parameter is not in the URL Query) we will try to access the context item (indicating that we are likely in a GraphQL call).

The solution presented below has been simplified to make it easier to read and understand. Once we have the home item, we just need to access the parent item to get the site.

```C#
public static Item GetContextHomeItem()
{
    // If the request came from "/sitecore/api/layout/render/jss", we get it from the HTTP context URL query,
    // otherwise (a.k.a. "/sitecore/api/graph/edge"), we get it from the Sitecore Context Item.

    const string key = "item";

    var url = HttpContext.Current.Request.Url;
    var uid = HttpUtility.ParseQueryString(url.Query).Get(key);

    if (!string.IsNullOrWhiteSpace(uid))
    {
        return Utilities.GetItem(uid);
    }

    var site = Context.Item?.GetRelativeSite();

    if (site == null)
    {
        return null;
    }

    return GetItem($"{site.Paths.Path}/Home");
}
```

## Conclusion

The main idea of this article is not just to present a proposed solution, which by the way can be improved in many aspects, but to help us remember that when dealing with JSS, we are possibly dealing with two fronts, with HTTP requests being made directly to the standard API and others made via GraphQL, and keeping an eye on these details can help us to create creative and reliable projects.

> You can access the complete source code used in this article through the [GitHub repository](https://github.com/andrelom/endless/blob/b6b50690a7f4cc8c0d991a3a3f5ab904aae97c4a/src/Foundation/Endless.Foundation.Core/Utilities.cs#L31).
