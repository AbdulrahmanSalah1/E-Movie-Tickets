// Initialize Swiper
const swiper = new Swiper('.swiper-container', {
    slidesPerView: 4,
    spaceBetween: 30,
    loop: true,
    lazy: true,
    navigation: {
        nextEl: '.swiper-button-next',
        prevEl: '.swiper-button-prev',
    },
    breakpoints: {
        1024: {
            slidesPerView: 4,
        },
        768: {
            slidesPerView: 2,
        },
        640: {
            slidesPerView: 1,
        },
    },
});
