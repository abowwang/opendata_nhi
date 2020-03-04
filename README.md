# 概述
> 跟風來寫段口罩庫存查詢功能，各大神提供的套件讓開發變得更輕鬆了

# 開發環境
DB : mysql:5.7.29

> 更新 appsetting.json - mydemo 中的連線設定

用到的兩張表 nhi_gauzemask & nhi_gauzemask_location 如下

```
CREATE TABLE `nhi_gauzemask` (
  `organization_id` varchar(15) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `organization_name` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `organization_addr` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `organization_tel` varchar(15) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `human_count` varchar(10) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `child_count` varchar(10) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `updated_at` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`organization_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

```
CREATE TABLE `nhi_gauzemask_location` (
  `organization_id` varchar(15) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `lat` varchar(15) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `lng` varchar(15) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `coord` point DEFAULT NULL,
  PRIMARY KEY (`organization_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

Framework : DotNet Core 3.1

Google API Key : 請至 Google Cloud Platform 申請

> 申請後更新 appsetting.json - GEOCODE_KEY

# 執行
定時刷新健保局口局庫存
>取得 健保特約機構口罩剩餘數量明細清單 http://{url}/home/GetMaskFromNHI

刷新沒有座標的藥局座標
>取得 健保特約機構口罩剩餘數量明細清單 http://{url}/home/GetEmptyLatLng

依座標取得最近的藥局列表
>取得 健保特約機構口罩剩餘數量明細清單 http://{url}/home/FindNearHealthOgranization?lat={latitude}&lng={longitude}

http://{url}/home/index
> 拖曳跳動的圖釘，刷新附近藥局 


以上..........