# PuntoSwitcherUkr для Ubuntu (Linux)

## Варіант 1: Скрипт (рекомендовано для швидкого запуску)
1. Встановіть системні утиліти:
   ```bash
   sudo apt update
   sudo apt install -y python3-pip xdotool xclip python3-gi gir1.2-gtk-3.0
   ```
2. Встановіть Python-залежності:
   ```bash
   cd /шлях/до/PuntoSwitcher2/linux
   pip3 install -r requirements.txt --user
   ```
3. Запуск:
   ```bash
   python3 punto_switcher_ukr.py &
   ```
   Програма з’явиться в треї.

## Варіант 2: Один файл через PyInstaller
1. Підготуйте залежності як у варіанті 1 + `pip3 install pyinstaller`.
2. У каталозі `linux` виконайте:
   ```bash
   pyinstaller --onefile --windowed punto_switcher_ukr.py
   ```
3. Передавайте файл `dist/punto_switcher_ukr` на інші машини Ubuntu.
4. На цільовій машині все одно потрібні утиліти:
   ```bash
   sudo apt install -y xdotool xclip
   ```
5. Запуск:
   ```bash
   ./punto_switcher_ukr &
   ```

## Використання
- За замовчуванням гаряча клавіша: Ctrl + Shift + F4 (міняється через пункт меню “Гаряча клавіша: … (змінити)” у треї).
- Щоб конвертувати текст: виділіть, натисніть гарячу клавішу — текст перетвориться EN↔UA залежно від напрямку.
- Трей-меню (ПКМ): перемкнути напрям (EN→UA / UA→EN), змінити гарячу клавішу, увімкнути/вимкнути автозапуск (desktop-файл у `~/.config/autostart/`), вийти.
- Налаштування зберігаються в `~/.config/punto_switcher_ukr/settings.json`.

## Потреби
- X-середовище (для pystray і xclip/xdotool).
- `xdotool` і `xclip` мають бути встановлені на цільовій системі для коректної роботи копіювання/вставки.
