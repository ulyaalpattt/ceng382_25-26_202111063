// Kullanıcı giriş bilgilerini saklamak için dizi
let loginAttempts = [];

// Saat güncelleme fonksiyonu
function updateClock() {
    const clockElement = document.getElementById('clock');
    const now = new Date();
    let hours = now.getHours();
    let minutes = now.getMinutes();
    let seconds = now.getSeconds();

    hours = hours < 10 ? '0' + hours : hours;
    minutes = minutes < 10 ? '0' + minutes : minutes;
    seconds = seconds < 10 ? '0' + seconds : seconds;

    const timeString = `${hours}:${minutes}:${seconds}`;
    clockElement.textContent = timeString;
}

// Sayfa yüklendiğinde saati güncelle ve her saniyede bir tekrar et
updateClock();
setInterval(updateClock, 1000);

// Form submit olayını dinle
document.getElementById("loginForm").addEventListener("submit", function(event) {
    event.preventDefault(); // Sayfanın yeniden yüklenmesini engelle

    // Kullanıcı adı ve şifreyi al
    let username = document.getElementById("username").value;
    let password = document.getElementById("password").value;

    // Kullanıcı giriş bilgilerini diziye ekle
    loginAttempts.push({ username: username, password: password });

    // Konsola yazdır
    console.log("Giriş Denemeleri:");
    console.table(loginAttempts);

    // Kullanıcıyı index2.html sayfasına yönlendir
    window.location.href = "table.html";
});

// Formları gizleme/gösterme özelliği
let formsVisible = true;

document.addEventListener('keydown', function(event) {
    if (event.key === 'H' || event.key === 'h') {
        const forms = document.querySelectorAll('form');
        forms.forEach(form => {
            form.style.display = formsVisible ? 'none' : 'block';
        });
        formsVisible = !formsVisible;
    }
});
