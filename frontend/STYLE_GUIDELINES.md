# 🎨 CCH Frontend Style Guidelines (CCH 前端視覺風格規範)

## ⚖️ 1. Core Philosophy (核心哲學)
本專案視覺風格必須與 **MyDimerco** 品牌語言保持高度一致。強調「專業、俐落、高辨識度」。
The visual style of this project must be highly consistent with the **MyDimerco** brand language. Emphasis on "Professional, Clean, and High Recognizability."

## 🎨 2. Color Palette (品牌調色盤)
請優先使用 `src/scss/style.scss` 定義的 CSS 變數：
Priority should be given to CSS variables defined in `src/scss/style.scss`:

- **Primary Blue (中菲行藍)**: `--primary-color` (`#00a8e2`) - 用於重點裝飾、主要按鈕、重要標籤。
- **Sidebar Gray (側欄深灰)**: `--sidebar-color` (`#465363`) - 用於標題、側邊選單背景。
- **Warning Orange (警告橘)**: `--warning-color` (`#f68b39`) - 用於 SLA 警告、異常狀態。
- **Dashboard BG (畫布背景)**: `--dashboard-bg` (`#f3f6f8`) - 全頁背景色。
- **Text Black (文字深藍)**: `--el-color-black` (`#011837`) - 主要文字顏色。

## ✍️ 3. Typography (字體規範)
- **Primary Font**: `MyDimerco-WorkSansBold` - 用於標題與強調文字。
- **Secondary Font**: `MyDimerco-OpenSansRegular` - 用於內文與說明文字。
- **Hierarchy (層級)**:
  - `h1`: 2rem, Bold, `--sidebar-color`.
  - `h3`: 1.25rem, Bold, `--sidebar-color`.
  - `subtitle`: 0.9rem, `#8898aa`.

## 📦 4. Component Standards (組件標準)
### 4.1 Cards (卡片樣式)
- **Border Radius**: `12px`.
- **Shadow**: `0 4px 12px rgba(0,0,0,0.05)`.
- **Hover Effect**: `transform: translateY(-4px); transition: 0.2s;`.

### 4.2 Decorators (區塊裝飾)
- 每個主要區塊 (`Section`) 標題左側應包含 **4px 寬的垂直裝飾線 (Decorator)**。
- 色彩應依區塊性質而定（Primary 或 Warning）。

### 4.3 Status Pills (狀態藥丸)
- 使用圓角 (`20px`) 背景色塊。
- 文字顏色固定為白色，確保對比度。

## 📏 5. Layout (佈局)
- **Container**: 最大寬度為 `1400px`，置中對齊。
- **Grid**: 狀態摘要採用 `grid-template-columns: repeat(auto-fill, minmax(200px, 1fr))`。
- **Spacing**: 優先使用 `$spacings` SCSS 變數。

---
*Created on 2026-04-09 by Gemini CLI*
