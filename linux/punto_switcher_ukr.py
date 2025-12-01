#!/usr/bin/env python3
import json
import os
import signal
import subprocess
import sys
import threading
import time
from pathlib import Path
from typing import Dict, Set, Tuple

from PIL import Image, ImageDraw
import pystray
from pynput import keyboard

# Шлях до конфігів / автозапуску
APP_NAME = "PuntoSwitcherUkr"
CONFIG_DIR = Path.home() / ".config" / "punto_switcher_ukr"
SETTINGS_PATH = CONFIG_DIR / "settings.json"
AUTOSTART_DIR = Path.home() / ".config" / "autostart"
AUTOSTART_FILE = AUTOSTART_DIR / "punto_switcher_ukr.desktop"

DEFAULT_SETTINGS = {
    "layout_direction": "en_to_ua",  # або "ua_to_en"
    "autostart": False,
    "hotkey": {"mods": ["ctrl", "shift"], "key": "f4"},
}

# Відповідність клавіш EN->UA
EN_TO_UA = {
    "`": "§",
    "q": "й",
    "w": "ц",
    "e": "у",
    "r": "к",
    "t": "е",
    "y": "н",
    "u": "г",
    "i": "ш",
    "o": "щ",
    "p": "з",
    "[": "х",
    "]": "ї",
    "a": "ф",
    "s": "і",
    "d": "в",
    "f": "а",
    "g": "п",
    "h": "р",
    "j": "о",
    "k": "л",
    "l": "д",
    ";": "ж",
    "'": "є",
    "z": "я",
    "x": "ч",
    "c": "с",
    "v": "м",
    "b": "и",
    "n": "т",
    "m": "ь",
    ",": "б",
    ".": "ю",
    "/": ".",
}
UA_TO_EN = {v: k for k, v in EN_TO_UA.items()}

pressed_keys: Set[str] = set()
hotkey_mods: Set[str] = set(DEFAULT_SETTINGS["hotkey"]["mods"])
hotkey_key: str = DEFAULT_SETTINGS["hotkey"]["key"]
layout_direction = DEFAULT_SETTINGS["layout_direction"]
autostart_enabled = DEFAULT_SETTINGS["autostart"]
listener = None
tray_icon = None


def load_settings():
    global layout_direction, autostart_enabled, hotkey_mods, hotkey_key
    try:
        if SETTINGS_PATH.exists():
            data = json.loads(SETTINGS_PATH.read_text())
        else:
            data = DEFAULT_SETTINGS
        layout_direction = data.get("layout_direction", DEFAULT_SETTINGS["layout_direction"])
        autostart_enabled = data.get("autostart", DEFAULT_SETTINGS["autostart"])
        hk = data.get("hotkey", DEFAULT_SETTINGS["hotkey"])
        hotkey_mods = set(hk.get("mods", ["ctrl", "shift"]))
        hotkey_key = hk.get("key", "p").lower()
    except Exception:
        layout_direction = DEFAULT_SETTINGS["layout_direction"]
        autostart_enabled = DEFAULT_SETTINGS["autostart"]
        hk = DEFAULT_SETTINGS["hotkey"]
        hotkey_mods = set(hk["mods"])
        hotkey_key = hk["key"]


def save_settings():
    CONFIG_DIR.mkdir(parents=True, exist_ok=True)
    data = {
        "layout_direction": layout_direction,
        "autostart": autostart_enabled,
        "hotkey": {"mods": list(hotkey_mods), "key": hotkey_key},
    }
    SETTINGS_PATH.write_text(json.dumps(data, indent=2))


def ensure_autostart(state: bool):
    AUTOSTART_DIR.mkdir(parents=True, exist_ok=True)
    if state:
        exe = Path(sys.argv[0]).resolve()
        AUTOSTART_FILE.write_text(
            f"[Desktop Entry]\n"
            f"Type=Application\n"
            f"Name={APP_NAME}\n"
            f"Exec=python3 {exe}\n"
            f"X-GNOME-Autostart-enabled=true\n"
        )
    else:
        if AUTOSTART_FILE.exists():
            AUTOSTART_FILE.unlink()


def key_to_token(key) -> str:
    if isinstance(key, keyboard.KeyCode):
        return (key.char or "").lower()
    if key in (keyboard.Key.ctrl_l, keyboard.Key.ctrl_r, keyboard.Key.ctrl):
        return "ctrl"
    if key in (keyboard.Key.shift_l, keyboard.Key.shift_r, keyboard.Key.shift):
        return "shift"
    if key in (keyboard.Key.alt_l, keyboard.Key.alt_r, keyboard.Key.alt):
        return "alt"
    function_map = {
        keyboard.Key.f1: "f1", keyboard.Key.f2: "f2", keyboard.Key.f3: "f3",
        keyboard.Key.f4: "f4", keyboard.Key.f5: "f5", keyboard.Key.f6: "f6",
        keyboard.Key.f7: "f7", keyboard.Key.f8: "f8", keyboard.Key.f9: "f9",
        keyboard.Key.f10: "f10", keyboard.Key.f11: "f11", keyboard.Key.f12: "f12",
    }
    if key in function_map:
        return function_map[key]
    return ""


def on_press(key):
    global pressed_keys
    token = key_to_token(key)
    if not token:
        return
    pressed_keys.add(token)
    if hotkey_key in pressed_keys and hotkey_mods.issubset(pressed_keys):
        # уникаємо повторних спрацювань, поки клавіші не відпущені
        if getattr(on_press, "fired", False):
            return
        on_press.fired = True
        threading.Thread(target=convert_selection, daemon=True).start()


def on_release(key):
    token = key_to_token(key)
    if token and token in pressed_keys:
        pressed_keys.discard(token)
    on_press.fired = False


def convert_selection():
    try:
        original_clip = get_clipboard()
    except Exception:
        original_clip = None

    # копіюємо виділення
    subprocess.run(["xdotool", "key", "ctrl+c"], stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
    time.sleep(0.05)

    try:
        text = get_clipboard()
    except Exception:
        return
    if not text.strip():
        return

    mapping = EN_TO_UA if layout_direction == "en_to_ua" else UA_TO_EN
    converted = convert_text(text, mapping)

    set_clipboard(converted)
    subprocess.run(["xdotool", "key", "ctrl+v"], stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)

    # відновлюємо буфер обміну, щоб не псувати користувачу
    if original_clip is not None:
        def restore():
            time.sleep(0.1)
            set_clipboard(original_clip)
        threading.Thread(target=restore, daemon=True).start()


def convert_text(text: str, mapping: Dict[str, str]) -> str:
    result = []
    for ch in text:
        lower = ch.lower()
        if lower in mapping:
            mapped = mapping[lower]
            result.append(mapped.upper() if ch.isupper() else mapped)
        else:
            result.append(ch)
    return "".join(result)


def get_clipboard() -> str:
    return subprocess.check_output(["xclip", "-o", "-selection", "clipboard"], text=True)


def set_clipboard(text: str):
    p = subprocess.Popen(["xclip", "-i", "-selection", "clipboard"], stdin=subprocess.PIPE, text=True)
    p.communicate(text)


def create_icon_image() -> Image.Image:
    size = 64
    img = Image.new("RGBA", (size, size), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    # клавіатура
    draw.rectangle([10, 30, 54, 50], fill=(240, 240, 240, 255), outline=(90, 90, 90, 255))
    for row in range(2):
        for col in range(8):
            x0 = 12 + col * 5
            y0 = 32 + row * 7
            draw.rectangle([x0, y0, x0 + 3, y0 + 3], fill=(200, 200, 200, 255))
    # подвійні стрілки
    draw.line([22, 18, 32, 10], fill=(60, 120, 210, 255), width=3)
    draw.line([42, 18, 32, 10], fill=(60, 120, 210, 255), width=3)
    draw.line([22, 10, 32, 18], fill=(60, 120, 210, 255), width=3)
    draw.line([42, 10, 32, 18], fill=(60, 120, 210, 255), width=3)
    return img


def hotkey_human() -> str:
    parts = []
    if "ctrl" in hotkey_mods:
        parts.append("Ctrl")
    if "shift" in hotkey_mods:
        parts.append("Shift")
    if "alt" in hotkey_mods:
        parts.append("Alt")
    parts.append(hotkey_key.upper())
    return "+".join(parts)


def toggle_layout():
    global layout_direction
    layout_direction = "ua_to_en" if layout_direction == "en_to_ua" else "en_to_ua"
    save_settings()
    tray_icon.title = f"{APP_NAME} ({hotkey_human()})"
    tray_icon.update_menu()


def toggle_autostart():
    global autostart_enabled
    autostart_enabled = not autostart_enabled
    ensure_autostart(autostart_enabled)
    save_settings()
    tray_icon.update_menu()


def set_hotkey_from_capture():
    # Простий захоплювач: слухає наступну комбінацію й зберігає
    event = threading.Event()
    captured: Tuple[Set[str], str] = (set(), "")

    def _on_press(k):
        token = key_to_token(k)
        if not token:
            return
        captured[0].add(token)
        if token not in {"ctrl", "shift", "alt"}:
            captured[1] = token

    def _on_release(k):
        if captured[1]:
            event.set()
            return False
        # завершуємо по Esc
        if k == keyboard.Key.esc:
            event.set()
            return False

    with keyboard.Listener(on_press=_on_press, on_release=_on_release) as cap_listener:
        event.wait(timeout=10)
        cap_listener.stop()

    if captured[1]:
        apply_new_hotkey(captured[0], captured[1])


def apply_new_hotkey(mods: Set[str], key: str):
    global hotkey_mods, hotkey_key, listener
    hotkey_mods = {m for m in mods if m in {"ctrl", "shift", "alt"}}
    hotkey_key = key
    save_settings()
    # перезапустити основний listener
    if listener:
        listener.stop()
    start_listener()
    tray_icon.title = f"{APP_NAME} ({hotkey_human()})"
    tray_icon.update_menu()


def start_listener():
    global listener
    listener = keyboard.Listener(on_press=on_press, on_release=on_release)
    listener.start()


def build_menu():
    return pystray.Menu(
        pystray.MenuItem(f"Перемкнути напрям (зараз: {'EN→UA' if layout_direction=='en_to_ua' else 'UA→EN'})",
                         lambda: toggle_layout()),
        pystray.MenuItem(f"Гаряча клавіша: {hotkey_human()} (змінити)", lambda: set_hotkey_from_capture()),
        pystray.MenuItem(f"Автозапуск: {'увімкнено' if autostart_enabled else 'вимкнено'}",
                         lambda: toggle_autostart()),
        pystray.MenuItem("Вийти", lambda: tray_icon.stop())
    )


def main():
    load_settings()
    ensure_autostart(autostart_enabled)
    start_listener()

    icon_img = create_icon_image()
    global tray_icon
    tray_icon = pystray.Icon(APP_NAME, icon_img, f"{APP_NAME} ({hotkey_human()})", build_menu())
    signal.signal(signal.SIGINT, lambda sig, frame: tray_icon.stop())
    tray_icon.run()


if __name__ == "__main__":
    main()
