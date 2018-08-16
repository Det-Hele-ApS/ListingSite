$(function () {

	"use strict";

	$(window).on('load', function () {
		$(".page_loader").fadeOut("fast");
	});

	$(window).on('beforeunload', function () {
		$(".page_loader").fadeIn('fast', function () {
			window.scroll(0, 0);
			$('body').css('overflow-y', 'hidden');
		});
	});

	adjustHeader();
	doSticky();

	$(window).on('scroll', function () {
		adjustHeader();
		doSticky();
	});

	function doSticky() {
		if ($(document).scrollTop() > 40) {
			$('.do-sticky').addClass('sticky-header');
		}
		else {
			$('.do-sticky').removeClass('sticky-header');
		}
	}

	function adjustHeader() {
		var windowWidth = $(window).width();
		if (windowWidth > 992) {
			if ($(document).scrollTop() >= 100) {
				if ($('.header-shrink').length < 1) {
					$('.sticky-header').addClass('header-shrink');
				}
				if ($('.do-sticky').length < 1) {
					$('.logo img').attr('src', '/images/logos/black-logo.png');
				}
			}
			else {
				$('.sticky-header').removeClass('header-shrink');
				if ($('.do-sticky').length < 1) {
					$('.logo img').attr('src', '/images/logos/logo.png');
				}
			}
		} else {
			$('.logo img').attr('src', '/images/logos/black-logo.png');
		}
	}

	// WOW animation library initialization
	var wow = new WOW(
		{
			animateClass: 'animated',
			offset: 100,
			mobile: false
		}
	);
	wow.init();

	$(".open-offcanvas, .close-offcanvas").on("click", function () {
		return $("body").toggleClass("off-canvas-sidebar-open"), !1
	});

	$(document).on("click", function (t) {
		var a = $(".off-canvas-sidebar");
		a.is(t.target) || 0 !== a.has(t.target).length || $("body").removeClass("off-canvas-sidebar-open")
	});

	function doAnimations(elems) {
		//Cache the animationend event in a variable
		var animEndEv = 'webkitAnimationEnd animationend';
		elems.each(function () {
			var $this = $(this),
				$animationType = $this.data('animation');
			$this.addClass($animationType).one(animEndEv, function () {
				$this.removeClass($animationType);
			});
		});
	}

	//Variables on page load
	var $myCarousel = $('#home-carousel');
	var $firstAnimatingElems = $myCarousel.find('.item:first').find("[data-animation ^= 'animated']");
	//Initialize carousel
	$myCarousel.carousel();

	//Animate captions in first slide on page load
	doAnimations($firstAnimatingElems);
	//Pause carousel
	$myCarousel.carousel('pause');
	//Other slides to be animated on carousel slide event
	$myCarousel.on('slide.bs.carousel', function (e) {
		var $animatingElems = $(e.relatedTarget).find("[data-animation ^= 'animated']");
		doAnimations($animatingElems);
	});

	$('#home-carousel').carousel({
		interval: 3000,
		pause: "false"
	});

	// DROPDOWN ON HOVER
	$(".dropdown").on('hover', function () {
		$('.dropdown-menu', this).stop().fadeIn("fast");
	},
		function () {
			$('.dropdown-menu', this).stop().fadeOut("fast");
		});

	// Dropdown activation
	$('.dropdown-menu a.dropdown-toggle').on('click', function (e) {
		if (!$(this).next().hasClass('show')) {
			$(this).parents('.dropdown-menu').first().find('.show').removeClass("show");
		}
		var $subMenu = $(this).next(".dropdown-menu");
		$subMenu.toggleClass('show');


		$(this).parents('li.nav-item.dropdown.show').on('hidden.bs.dropdown', function (e) {
			$('.dropdown-submenu .show').removeClass("show");
		});

		return false;
	});

	// Page scroller initialization.
	$.scrollUp({
		scrollName: 'page_scroller',
		scrollDistance: 300,
		scrollFrom: 'top',
		scrollSpeed: 500,
		easingType: 'linear',
		animation: 'fade',
		animationSpeed: 200,
		scrollTrigger: false,
		scrollTarget: false,
		scrollText: '<i class="fa fa-chevron-up"></i>',
		scrollTitle: false,
		scrollImg: false,
		activeOverlay: false,
		zIndex: 2147483647
	});


	// Magnify activation
	$('.property-magnify-gallery').each(function () {
		$(this).magnificPopup({
			delegate: 'a',
			type: 'image',
			gallery: { enabled: true }
		});
	});

	$(".range-slider-ui").each(function () {
		var minRangeValue = $(this).attr('data-min');
		var maxRangeValue = $(this).attr('data-max');
		var minName = $(this).attr('data-min-name');
		var maxName = $(this).attr('data-max-name');
		var unit = $(this).attr('data-unit');

		$(this).append("" +
			"<span class='min-value'></span> " +
			"<span class='max-value'></span>" +
			"<input class='current-min' type='hidden' name='" + minName + "'>" +
			"<input class='current-max' type='hidden' name='" + maxName + "'>"
		);
		$(this).slider({
			range: true,
			min: minRangeValue,
			max: maxRangeValue,
			values: [minRangeValue, maxRangeValue],
			slide: function (event, ui) {
				event = event;
				var currentMin = parseInt(ui.values[0], 10);
				var currentMax = parseInt(ui.values[1], 10);
				$(this).children(".min-value").text(currentMin + " " + unit);
				$(this).children(".max-value").text(currentMax + " " + unit);
				$(this).children(".current-min").val(currentMin);
				$(this).children(".current-max").val(currentMax);
			}
		});

		var currentMin = parseInt($(this).slider("values", 0), 10);
		var currentMax = parseInt($(this).slider("values", 1), 10);
		$(this).children(".min-value").text(currentMin + " " + unit);
		$(this).children(".max-value").text(currentMax + " " + unit);
		$(this).children(".current-min").val(currentMin);
		$(this).children(".current-max").val(currentMax);
	});

	$('select').selectBox({
		mobile: true,
	});
});