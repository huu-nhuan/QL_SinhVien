
function validatePasswords() {
    var password = document.getElementById("MatKhau").value;
    var confirmPassword = document.getElementsByName("confirmPassword")[0].value;
    if (password !== confirmPassword) {
        alert("Passwords do not match!");
        return false;
    }
    return true;
}