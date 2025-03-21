let loginAttempts = [];

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

updateClock();
setInterval(updateClock, 1000);

document.getElementById("loginForm").addEventListener("submit", function(event) {
    event.preventDefault(); 

    let username = document.getElementById("username").value;
    let password = document.getElementById("password").value;

    loginAttempts.push({ username: username, password: password });

    console.log("Giriş Denemeleri:");
    console.table(loginAttempts);

    if (username === "admin" && password === "admin") {
        window.location.href = "table.html"; 
    } else {
        alert("Hatalı kullanıcı adı veya şifre!");
    }
});



document.getElementById("forgotPassword").addEventListener("click", function(){
    alert("Password reset link has been sent to your email address!");
});


document.getElementById("signUp").addEventListener("click", function() {
    window.location.href = "signup.html"; 
});

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

