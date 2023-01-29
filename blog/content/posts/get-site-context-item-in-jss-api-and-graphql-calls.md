---
author: 'André Moraes'
title: 'Get site context item in JSS API and GraphQL calls'
description: 'How to get corresponding site context item in JSS API and GraphQL calls.'
tags: []
date: 2023-01-28T21:16:50-03:00
draft: false
---

In some projects, we are faced with the need to create new pipelines or even a new service layout for applications using Sitecore JSS. In these situations it is common to have access to the site context item.

The problem is that many times these pipelines or layout services are used either for API calls (a.k.a. "/sitecore/api/layout/render/jss") or GraphQL (a.k.a. "/sitecore/api/graph/edge"). In the execution context of both situations, access to the context item alternates, and in API-type calls, the object will not point to the website item we expect.

To solve this problem, we can create a method that will validate if the call is made through the JSS API or GraphQL, and return the relative site context item for both situations.

## Get to Work

First, we need to ensure that we will have access to the site item relative to any item we have access to, this way, regardless of the item we get from the context, we will know which site it belongs to.

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

With this method in hand (which, by the way, can be useful in many other situations), we can create the methods that will help us to access the item site.

```C#
public static Item GetContextHomeItem()
{
    // If the request came from "/sitecore/api/layout/render/jss", we get it from the HTTP context URL query,
    // otherwise (a.k.a. "/sitecore/api/graph/edge"), we get it from the Sitecore Context Item.
    return GetHttpContextHomeItem() ?? GetSitecoreContextHomeItem();
}

public static Item GetSitecoreContextHomeItem()
{
    var site = Context.Item?.GetRelativeSite();

    if (site == null)
    {
        return null;
    }

    return GetItem($"{site.Paths.Path}/Home");
}

public static Item GetHttpContextHomeItem()
{
    const string key = "item";

    var url = HttpContext.Current.Request.Url;
    var uid = HttpUtility.ParseQueryString(url.Query).Get(key);

    if (string.IsNullOrWhiteSpace(uid))
    {
        return null;
    }

    return GetItem(uid);
}
```
