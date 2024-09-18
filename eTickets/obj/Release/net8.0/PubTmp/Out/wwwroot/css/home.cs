@model IEnumerable<Cinema>

<head>
    <link rel="stylesheet" href="https://unpkg.com/swiper/swiper-bundle.min.css" />
    <style>
        /* Custom styles */
        .swiper-container {
            width: 100%;
            height: 70vh; /* Height for landscape view */
            padding: 20px 0;
            background-color: #f8f9fa; /* Light background for contrast */
        }

        .swiper-slide {
            display: flex;
            flex-direction: column;
            align-items: center;
            text-align: center;
            height: 100%;
            background-color: #ffffff; /* Card background color */
            border-radius: 10px;
            box-shadow: 0 4px 8px rgba(0,0,0,0.2); /* Card shadow */
        }

        .cinema-title {
            margin-bottom: 20px;
            font-size: 2rem;
            font-weight: bold;
            color: #007bff; /* Cinematic blue */
            text-transform: uppercase;
        }

        .movie-swiper-container {
            width: 100%;
            height: calc(100% - 80px); /* Adjust height to fit within the swiper-slide */
        }

        .swiper-slide .card {
            width: 250px;
            margin: 10px;
            border-radius: 10px;
            overflow: hidden;
            box-shadow: 0 4px 8px rgba(0,0,0,0.2);
            flex: 0 0 auto;
        }

        .card img {
            object-fit: cover;
            width: 100%;
            height: 150px; /* Adjust for landscape */
        }

        .card-body {
            padding: 10px;
            background-color: #ffffff; /* Card background */
        }

        .swiper-button-next, .swiper-button-prev {
            color: #007bff; /* Match cinema title color */
            background-color: rgba(255, 255, 255, 0.8); /* Slightly transparent background */
            border-radius: 50%;
            width: 40px;
            height: 40px;
        }

        .swiper-pagination-bullet {
            background-color: #007bff; /* Match cinema title color */
        }

        .badge-icon {
            margin-right: 5px;
        }
    </style>
</head>

<body>
    <div class="container mt-4">
        @foreach (var cinema in Model)
        {
            <div class="swiper-container">
                <div class="swiper-wrapper">
                    <div class="swiper-slide">
                        <div class="cinema-title"><a asp-controller="Cinemas" asp-action="Detail" asp-route-id="@cinema.Id">@cinema.Name</a></div>
                        <div class="movie-swiper-container">
                            <div class="swiper-container movie-slider">
                                <div class="swiper-wrapper">
                                    @foreach (var movie in cinema.Movies)
                                    {
                                        <div class="swiper-slide">
                                            <div class="card h-100">
                                                <a asp-controller="Movies" asp-action="Details" asp-route-id="@movie.Id">
                                                    <img src="@movie.ImageURL" alt="@movie.Name" class="img-fluid" />
                                                </a>
                                                <div class="card-body">
                                                    <h5 class="card-title">@movie.Name</h5>
                                                    <p class="card-text">
                                                        <strong>Status:</strong>&emsp;
                                                        @if (DateTime.Now >= movie.StartDate && DateTime.Now <= movie.EndDate)
                                                        {
                                                            <span class="badge badge-success">
                                                                <i class="bi bi-check-circle badge-icon"></i> AVAILABLE
                                                            </span>
                                                        }
                                                        else if (DateTime.Now > movie.EndDate)
                                                        {
                                                            <span class="badge badge-danger">
                                                                <i class="bi bi-x-circle badge-icon"></i> EXPIRED
                                                            </span>
                                                        }
                                                        else
                                                        {
                                                            <span class="badge badge-primary">
                                                                <i class="bi bi-clock badge-icon"></i> UPCOMING
                                                            </span>
                                                        }
                                                    </p>
                                                </div>
                                                <div class="card-footer">
                                                    @if (!User.IsInRole("Admin"))
                                                    {
                                                        <a class="btn btn-success text-white @((DateTime.Now > movie.EndDate) ? "disabled" : "")"
                                                           asp-controller="Orders"
                                                           asp-action="AddItemToShoppingCart"
                                                           asp-route-id="@movie.Id">
                                                            <i class="bi bi-cart-plus"></i> Add to Cart (Price @movie.Price.ToString("c"))
                                                        </a>
                                                    }
                                                </div>
                                            </div>
                                        </div>
                                    }
                                </div>
                                <!-- Add Pagination -->
                                <div class="swiper-pagination"></div>

                                <!-- Add Navigation -->
                                <div class="swiper-button-next"></div>
                                <div class="swiper-button-prev"></div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        }
    </div>

    <script src="https://unpkg.com/swiper/swiper-bundle.min.js"></script>
    <script>
        document.addEventListener('DOMContentLoaded', function () {
            // Initialize Swiper for each cinema's movie slider
            document.querySelectorAll('.movie-slider').forEach(function (slider) {
                new Swiper(slider, {
                    slidesPerView: 'auto',
                    spaceBetween: 20,
                    loop: true,
                    pagination: {
                        el: '.swiper-pagination',
                        clickable: true,
                    },
                    navigation: {
                        nextEl: '.swiper-button-next',
                        prevEl: '.swiper-button-prev',
                    },
                    breakpoints: {
                        640: {
                            slidesPerView: 'auto',
                            spaceBetween: 20,
                        },@model IEnumerable<Cinema>

<head>
    <link href="https://fonts.googleapis.com/css2?family=Lato:wght@300;400;700&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="https://unpkg.com/swiper/swiper-bundle.min.css" />
    <style>
        body {
            font-family: 'Lato', sans-serif;
            background-color: #f0f2f5;
        }

        .swiper-container {
            width: 100%;
            height: 70vh;
            padding: 20px 0;
            background-color: #ffffff;
        }

        .swiper-slide {
            display: flex;
            flex-direction: column;
            align-items: center;
            text-align: center;
            height: 100%;
            background-color: #ffffff;
            border-radius: 10px;
            box-shadow: 0 6px 12px rgba(0, 0, 0, 0.15);
            transition: transform 0.3s ease;
        }

        .swiper-slide:hover {
            transform: translateY(-5px);
        }

        .cinema-title {
            margin-bottom: 20px;
            font-size: 2rem;
            font-weight: bold;
            color: #007bff;
            text-transform: uppercase;
        }

        .cinema-title a {
            text-decoration: none;
            color: inherit;
            transition: color 0.3s;
        }

        .cinema-title a:hover {
            color: #0056b3;
        }

        .movie-swiper-container {
            width: 100%;
            height: calc(100% - 80px);
        }

        .swiper-slide .card {
            width: 250px;
            margin: 10px;
            border-radius: 10px;
            overflow: hidden;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
            flex: 0 0 auto;
            transition: transform 0.3s ease;
        }

        .swiper-slide .card:hover {
            transform: translateY(-5px);
        }

        .card img {
            object-fit: cover;
            width: 100%;
            height: 150px;
        }

        .card-body {
            padding: 10px;
            background-color: #ffffff;
        }

        .card-footer {
            padding: 10px;
            background-color: #f8f9fa;
            text-align: center;
        }

        .swiper-button-next, .swiper-button-prev {
            color: #007bff;
            background-color: rgba(255, 255, 255, 0.8);
            border-radius: 50%;
            width: 40px;
            height: 40px;
        }

        .swiper-pagination-bullet {
            background-color: #007bff;
        }

        .badge-icon {
            margin-right: 5px;
        }

        .btn-success:hover {
            background-color: #218838;
            border-color: #1e7e34;
        }
    </style>
</head>

<body>
    <div class="container mt-4">
        @foreach (var cinema in Model)
        {
            <div class="swiper-container">
                <div class="swiper-wrapper">
                    <div class="swiper-slide">
                        <div class="cinema-title">
                            <a asp-controller="Cinemas" asp-action="Detail" asp-route-id="@cinema.Id">@cinema.Name</a>
                        </div>
                        <div class="movie-swiper-container">
                            <div class="swiper-container movie-slider">
                                <div class="swiper-wrapper">
                                    @foreach (var movie in cinema.Movies)
                                    {
                                        <div class="swiper-slide">
                                            <div class="card h-100">
                                                <a asp-controller="Movies" asp-action="Details" asp-route-id="@movie.Id">
                                                    <img src="@movie.ImageURL" alt="@movie.Name" class="img-fluid" />
                                                </a>
                                                <div class="card-body">
                                                    <h5 class="card-title">@movie.Name</h5>
                                                    <p class="card-text">
                                                        <strong>Status:</strong>&emsp;
                                                        @if (DateTime.Now >= movie.StartDate && DateTime.Now <= movie.EndDate)
                                                        {
                                                            <span class="badge badge-success">
                                                                <i class="bi bi-check-circle badge-icon"></i> AVAILABLE
                                                            </span>
                                                        }
                                                        else if (DateTime.Now > movie.EndDate)
                                                        {
                                                            <span class="badge badge-danger">
                                                                <i class="bi bi-x-circle badge-icon"></i> EXPIRED
                                                            </span>
                                                        }
                                                        else
                                                        {
                                                            <span class="badge badge-primary">
                                                                <i class="bi bi-clock badge-icon"></i> UPCOMING
                                                            </span>
                                                        }
                                                    </p>
                                                </div>
                                                <div class="card-footer">
                                                    @if (!User.IsInRole("Admin"))
                                                    {
                                                        <a class="btn btn-success text-white @((DateTime.Now > movie.EndDate) ? "disabled" : "")"
                                                           asp-controller="Orders"
                                                           asp-action="AddItemToShoppingCart"
                                                           asp-route-id="@movie.Id">
                                                            <i class="bi bi-cart-plus"></i>

                        768: {
                            slidesPerView: 'auto',
                            spaceBetween: 30,
                        },
                        1024: {
                            slidesPerView: 'auto',
                            spaceBetween: 40,
                        },
                    },
                });
            });
        });
    </script>
</body>
enhancement become professional gui