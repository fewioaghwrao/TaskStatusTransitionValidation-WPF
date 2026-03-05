# Configuration

## appsettings
- `appsettings.json`：必須（Git管理）
- `appsettings.Development.json`：ローカル用（任意）

`MainWindow` で `DOTNET_ENVIRONMENT` を読み込み、環境別設定をロードします。

## Api BaseUrl
`appsettings.json` の例：

```json
{
  "Api": {
    "BaseUrl": "https://example.com/"
  }
}
```

## CI build note

CI環境では appsettings.Development.json が存在しないため、
csproj側で Exists() 条件を付けてコピー対象を任意化しています。

---