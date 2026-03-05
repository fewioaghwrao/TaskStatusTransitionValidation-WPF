# Testing

## Test policy
UI（XAML）自体は壊れやすくコストが高いため、  
本プロジェクトでは **ViewModel / Service / Session** を中心にユニットテストします。

## Target
- `AppSession`
  - Clear() の動作
  - IsLeader の判定（大小文字吸収）
- `LoginViewModel`
  - CanExecute（入力必須/Busy制御）
  - 成功時：Token/Me が Session に格納されイベントが発火
  - 失敗時：ErrorMessage が設定され Busy が復帰

## Run locally
```bash
dotnet test
```
## CI
GitHub Actions で dotnet build / dotnet test を自動実行します。

---
