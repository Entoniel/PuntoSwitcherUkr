# PuntoSwitcherUkr для Windows

## Готовий exe (самодостатній)
1. Запустіть `publish.ps1` у корені проєкту (`C:\458_PuntoSwitcher2\PuntoSwitcher2`): ПКМ → Run with PowerShell або  
   `powershell -ExecutionPolicy Bypass -File .\publish.ps1`
2. Отриманий файл: `publish\win-x64\PuntoSwitcherUkr.exe` (плюс решта файлів у цій папці). Цю папку можна скопіювати на інший ПК і запускати без встановленого .NET.
3. Запуск: двічі клікніть `PuntoSwitcherUkr.exe`. Програма сховається в трей.

## Інсталятор
1. Відкрийте `installer\punto_switcher2.iss` у Inno Setup Compiler.
2. Натисніть Compile (F9). Буде зібрано `PuntoSwitcherUkr_Setup.exe` у цій самій папці.
3. Передайте й запустіть `PuntoSwitcherUkr_Setup.exe` на цільовому ПК. Майстер встановлення створить ярлик і (за бажанням) автозапуск.

## Використання
- За замовчуванням гаряча клавіша: Ctrl + Shift + F4 (можна змінити в налаштуваннях).
- Щоб конвертувати текст: виділіть його, натисніть гарячу клавішу — текст перетвориться EN↔UA згідно вибраного напрямку.
- Трей: ЛКМ відкриває налаштування; ПКМ — меню “Налаштування” / “Вихід”.
- Налаштування: напрямок розкладки, автозапуск, зміна гарячої клавіші.

## Де зберігаються дані
- Налаштування: `%AppData%\PuntoSwitcherUkr\settings.json`
- Автозапуск: запис у HKCU\Software\Microsoft\Windows\CurrentVersion\Run
