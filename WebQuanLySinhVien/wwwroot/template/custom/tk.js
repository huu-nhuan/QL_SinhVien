
document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll(".toggle-password").forEach(button => {
        button.addEventListener("click", function () {
            let row = this.closest("td");
            let hiddenPassword = row.querySelector(".password-hidden");
            let plainPassword = row.querySelector(".password-plain");

            if (plainPassword.classList.contains("d-none")) {
                plainPassword.classList.remove("d-none");
                hiddenPassword.classList.add("d-none");
                this.textContent = "Ẩn mật khẩu";
            } else {
                plainPassword.classList.add("d-none");
                hiddenPassword.classList.remove("d-none");
                this.textContent = "Hiện mật khẩu";
            }
        });
    });
});

