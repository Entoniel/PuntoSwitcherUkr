# PuntoSwitcherUkr

Відкрита альтернатива класичному Punto Switcher: конвертує виділений текст між EN↔UA гарячою клавішею і працює з трею. Код прозорий, без прихованих функцій, аби не хвилюватися, що перехоплення клавіш потрапить кудись зайве.

> Ідея надихнута старою російською програмою Punto Switcher, але тут повністю відкритий код і відсутні сторонні дзвінки чи приховані сервіси — усе можна перевірити.

## Можливості
- Гаряча клавіша за замовчуванням: Ctrl+Shift+F4 (налаштовується).
- Перемикання напрямку EN→UA або UA→EN.
- Трей-меню: налаштування, автозапуск, вихід.
- Зберігання налаштувань у користувацькому профілі.

## Збірки
- Windows: див. `Win/README.md` — самодостатній `PuntoSwitcherUkr.exe` або інсталятор `PuntoSwitcherUkr_Setup.exe`.
- Linux (Ubuntu): див. `Linux/README.md` — Python-скрипт або одиночний файл через PyInstaller.

## Структура
- `PuntoSwitcher2/` — код WinForms (.NET 6, Windows).
- `linux/` — версія для Ubuntu (Python, pynput, pystray).
- `Win/README.md`, `Linux/README.md` — інструкції для кожної платформи.

## Як опублікувати на GitHub
1. Створіть новий репозиторій на GitHub (наприклад, `PuntoSwitcherUkr`).
2. У корені проєкту виконайте:
   ```bash
   git init
   git add .
   git commit -m "Initial commit: PuntoSwitcherUkr for Windows and Linux"
   git remote add origin https://github.com/<your-user>/PuntoSwitcherUkr.git
   git push -u origin main
   ```
3. Додайте релізи з артефактами:
   - Для Windows: зберіть інсталятор (`PuntoSwitcherUkr_Setup.exe`) або покладіть `publish/win-x64/PuntoSwitcherUkr.exe`.
   - Для Linux: зберіть через PyInstaller (`dist/punto_switcher_ukr`) або додайте скрипт і `requirements.txt`.

## Ліцензія
Додайте потрібну ліцензію (наприклад, MIT або Apache-2.0) перед публікацією — файл `LICENSE` можна підключити за потреби.
