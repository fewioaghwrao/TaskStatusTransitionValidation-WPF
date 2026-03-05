# Architecture（WPF / MVVM）

## Overview
本アプリは ASP.NET Core Web API を利用する WPFデスクトップクライアントです。  
業務ルール（状態遷移/権限）は API 側に集約し、WPF は UI と API 呼び出しに集中します。

## Layering
- Views：画面（XAML）
- ViewModels：画面ロジック（状態・コマンド・画面遷移イベント）
- Services：API通信・セッション保持（`ApiClient` / `AppSession`）

```bash
Views
↓ (Binding)
ViewModels
↓ (call)
Services
↓
REST API
```


## Key classes
- `AppSession`
  - ログイン済みユーザー情報（Me）とトークンを保持
  - `IsLeader` でロール判定（UI制御に利用）
- `IApiClient`
  - ViewModel をテスト可能にするための抽象化
- `ApiClient`
  - `HttpClient` を利用して REST API を呼び出し
  - `ProblemDetails` を可能な範囲で整形して例外化

## Navigation
MainWindow は `Host.Content` を切り替えて画面遷移します。
- Login → Projects → Tasks → TaskDetail
- Logout で Login に戻す