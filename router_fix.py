import os

# --- الإعدادات (بدلهم كيف بغيتي) ---
SSID_NAME = "WiFi_OMAR_PRIVATE"
PASSWORD  = "Omar2026Secure"

def run(cmd):
    return os.system(cmd)

print("="*50)
print("[+] جاري إعداد المصدر المستقل عبر الـ CMD...")
print("="*50)

# 1. تبديل السمية والباسورد ف السيستم (بزز منه)
# هاد الأمر كيدخل ديريكت لإعدادات الـ Hotspot
run(f'netsh wlan set hostednetwork mode=allow ssid="{SSID_NAME}" key="{PASSWORD}"')

# 2. تفعيل "الجدار الحديدي" (Firewall)
# هاد الجزء هو اللي كيحمي ليك وثائقك السرية فاش كتكون كتصفح
print("[+] تفعيل الحماية والجدار الحديدي...")
run('netsh advfirewall firewall set rule group="Network Discovery" new enable=No')
run('netsh advfirewall firewall add rule name="Block_Inbound" dir=in action=block protocol=ANY profile=any')

# 3. فتح نافذة الإعدادات باش تأكد بلي "On" شاعلة
print("[!] غتفتح ليك دابا نافذة الإعدادات.. تأكد بلي Mobile Hotspot راها (On)")
os.system('start ms-settings:network-mobilehotspot')

print("="*50)
print(f"[*] كولشي ناضي دابا!")
print(f"[*] SSID: {SSID_NAME}")
print(f"[*] PASS: {PASSWORD}")
print("="*50)

input("\n[!] خلي هاد النافذة مفتوحة... وبرك على Enter يلا بغيتي تطفي كلشي.")