document.querySelectorAll('.star-label').forEach(label => {
    label.addEventListener('click', function() {
        const starIcons = document.querySelectorAll('.star-icon');
        starIcons.forEach(icon => icon.style.color = '#ccc');
        
        this.querySelector('.star-icon').style.color = '#ffc107';
        let previousLabel = this.previousElementSibling;
        
        while (previousLabel) {
            previousLabel.querySelector('.star-icon').style.color = '#ffc107';
            previousLabel = previousLabel.previousElementSibling;
        }
    });
});