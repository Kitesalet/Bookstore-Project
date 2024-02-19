$(document).ready(function () {
    let companyDropdown = document.getElementById('companyDropdown');
    let roleDropdown = document.getElementById('roleDropdown');

    roleDropdown.addEventListener('change', function () {
        if (this.value === 'Employee') {
            companyDropdown.classList.remove('d-none');
        } else {
            companyDropdown.classList.add('d-none');
        }
    })
})

