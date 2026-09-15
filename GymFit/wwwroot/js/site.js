
// Document Ready
$(document).ready(function () {
    // Initialize tooltips
    initializeTooltips();

    // Initialize popovers
    initializePopovers();

    // Smooth scrolling
    initializeSmoothScroll();

    // Form validation
    initializeFormValidation();

    // Navbar scroll effect
    initializeNavbarScroll();

    // Auto-hide alerts
    autoHideAlerts();

    // Animate on scroll
    initializeScrollAnimations();

    // Mobile menu
    initializeMobileMenu();

    console.log('GymFit initialized successfully');
});

// Initialize Bootstrap Tooltips
function initializeTooltips() {
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
}

// Initialize Bootstrap Popovers
function initializePopovers() {
    const popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
    popoverTriggerList.map(function (popoverTriggerEl) {
        return new bootstrap.Popover(popoverTriggerEl);
    });
}

// Smooth Scrolling for Anchor Links
function initializeSmoothScroll() {
    $('a[href*="#"]:not([href="#"])').click(function () {
        if (location.pathname.replace(/^\//, '') == this.pathname.replace(/^\//, '')
            && location.hostname == this.hostname) {
            let target = $(this.hash);
            target = target.length ? target : $('[name=' + this.hash.slice(1) + ']');
            if (target.length) {
                $('html, body').animate({
                    scrollTop: target.offset().top - 70
                }, 1000);
                return false;
            }
        }
    });
}

// Form Validation Enhancement
function initializeFormValidation() {
    // Real-time validation
    $('input[required], textarea[required], select[required]').on('blur', function () {
        validateField($(this));
    });

    // Password strength indicator
    $('input[type="password"]').on('input', function () {
        checkPasswordStrength($(this));
    });

    // Email validation
    $('input[type="email"]').on('blur', function () {
        validateEmail($(this));
    });

    // Phone number formatting
    $('input[type="tel"]').on('input', function () {
        formatPhoneNumber($(this));
    });
}

// Validate Individual Field
function validateField(field) {
    const value = field.val().trim();
    const fieldName = field.attr('name') || 'This field';

    // Clear previous validation
    field.removeClass('is-invalid is-valid');
    field.siblings('.invalid-feedback').remove();

    // Required validation
    if (field.prop('required') && !value) {
        markFieldInvalid(field, fieldName + ' is required');
        return false;
    }

    // Length validation
    const minLength = field.attr('minlength');
    if (minLength && value.length < minLength) {
        markFieldInvalid(field, `Minimum ${minLength} characters required`);
        return false;
    }

    const maxLength = field.attr('maxlength');
    if (maxLength && value.length > maxLength) {
        markFieldInvalid(field, `Maximum ${maxLength} characters allowed`);
        return false;
    }

    // Mark as valid
    field.addClass('is-valid');
    return true;
}

// Mark Field as Invalid
function markFieldInvalid(field, message) {
    field.addClass('is-invalid');
    field.after(`<div class="invalid-feedback">${message}</div>`);
}

// Email Validation
function validateEmail(field) {
    const email = field.val().trim();
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

    field.removeClass('is-invalid is-valid');
    field.siblings('.invalid-feedback').remove();

    if (email && !emailRegex.test(email)) {
        markFieldInvalid(field, 'Please enter a valid email address');
        return false;
    }

    if (email) {
        field.addClass('is-valid');
    }
    return true;
}

// Password Strength Checker
function checkPasswordStrength(field) {
    const password = field.val();
    let strength = 0;
    let feedback = '';

    // Remove existing feedback
    field.siblings('.password-strength').remove();

    if (password.length >= 8) strength++;
    if (password.match(/[a-z]/)) strength++;
    if (password.match(/[A-Z]/)) strength++;
    if (password.match(/[0-9]/)) strength++;
    if (password.match(/[^a-zA-Z0-9]/)) strength++;

    let strengthClass = '';
    let strengthText = '';

    switch (strength) {
        case 0:
        case 1:
            strengthClass = 'text-danger';
            strengthText = 'Weak';
            break;
        case 2:
        case 3:
            strengthClass = 'text-warning';
            strengthText = 'Medium';
            break;
        case 4:
        case 5:
            strengthClass = 'text-success';
            strengthText = 'Strong';
            break;
    }

    if (password.length > 0) {
        field.after(`<small class="password-strength ${strengthClass}">Password Strength: ${strengthText}</small>`);
    }
}

// Phone Number Formatting
function formatPhoneNumber(field) {
    let value = field.val().replace(/\D/g, '');

    if (value.length > 10) {
        value = value.substring(0, 10);
    }

    if (value.length >= 6) {
        value = value.replace(/(\d{3})(\d{3})(\d{0,4})/, '($1) $2-$3');
    } else if (value.length >= 3) {
        value = value.replace(/(\d{3})(\d{0,3})/, '($1) $2');
    }

    field.val(value);
}

// Navbar Scroll Effect
function initializeNavbarScroll() {
    $(window).scroll(function () {
        if ($(this).scrollTop() > 50) {
            $('.navbar').addClass('navbar-scrolled');
        } else {
            $('.navbar').removeClass('navbar-scrolled');
        }
    });
}

// Auto-hide Alerts
function autoHideAlerts() {
    $('.alert').each(function () {
        const alert = $(this);
        if (!alert.hasClass('alert-permanent')) {
            setTimeout(function () {
                alert.fadeOut('slow', function () {
                    alert.remove();
                });
            }, 5000);
        }
    });
}

// Scroll Animations
function initializeScrollAnimations() {
    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('fade-in');
                observer.unobserve(entry.target);
            }
        });
    }, {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    });

    // Observe elements
    document.querySelectorAll('.card, .feature-box, .pricing-card, .stat-card').forEach(el => {
        observer.observe(el);
    });
}

// Mobile Menu Toggle
function initializeMobileMenu() {
    $('.navbar-toggler').on('click', function () {
        $('body').toggleClass('menu-open');
    });

    // Close menu when clicking outside
    $(document).on('click', function (e) {
        if (!$(e.target).closest('.navbar').length) {
            $('.navbar-collapse').removeClass('show');
            $('body').removeClass('menu-open');
        }
    });
}

// Confirm Dialog Helper
function confirmAction(message, callback) {
    if (confirm(message)) {
        callback();
    }
}

// Loading Indicator
function showLoading(element) {
    const loadingHtml = '<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>Loading...';
    const originalHtml = element.html();
    element.data('original-html', originalHtml);
    element.html(loadingHtml);
    element.prop('disabled', true);
}

function hideLoading(element) {
    const originalHtml = element.data('original-html');
    element.html(originalHtml);
    element.prop('disabled', false);
}

// Number Formatting
function formatCurrency(amount) {
    return '$' + parseFloat(amount).toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');
}

function formatNumber(num) {
    return num.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
}

// Date Formatting
function formatDate(dateString) {
    const date = new Date(dateString);
    const options = { year: 'numeric', month: 'short', day: 'numeric' };
    return date.toLocaleDateString('en-US', options);
}

function formatDateTime(dateString) {
    const date = new Date(dateString);
    const dateOptions = { year: 'numeric', month: 'short', day: 'numeric' };
    const timeOptions = { hour: '2-digit', minute: '2-digit' };
    return date.toLocaleDateString('en-US', dateOptions) + ' ' + date.toLocaleTimeString('en-US', timeOptions);
}

// Local Storage Helper
const storage = {
    set: function (key, value) {
        try {
            localStorage.setItem(key, JSON.stringify(value));
            return true;
        } catch (e) {
            console.error('Storage error:', e);
            return false;
        }
    },

    get: function (key) {
        try {
            const item = localStorage.getItem(key);
            return item ? JSON.parse(item) : null;
        } catch (e) {
            console.error('Storage error:', e);
            return null;
        }
    },

    remove: function (key) {
        try {
            localStorage.removeItem(key);
            return true;
        } catch (e) {
            console.error('Storage error:', e);
            return false;
        }
    },

    clear: function () {
        try {
            localStorage.clear();
            return true;
        } catch (e) {
            console.error('Storage error:', e);
            return false;
        }
    }
};

// Debounce Function
function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}

// Copy to Clipboard
function copyToClipboard(text) {
    const textarea = document.createElement('textarea');
    textarea.value = text;
    textarea.style.position = 'fixed';
    textarea.style.opacity = '0';
    document.body.appendChild(textarea);
    textarea.select();

    try {
        document.execCommand('copy');
        showToast('success', 'Copied to clipboard!');
    } catch (err) {
        showToast('error', 'Failed to copy');
    }

    document.body.removeChild(textarea);
}

// Print Function
function printElement(elementId) {
    const printContents = document.getElementById(elementId).innerHTML;
    const originalContents = document.body.innerHTML;

    document.body.innerHTML = printContents;
    window.print();
    document.body.innerHTML = originalContents;
    location.reload();
}

// Download File
function downloadFile(url, filename) {
    const link = document.createElement('a');
    link.href = url;
    link.download = filename;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

// Check Internet Connection
function checkConnection() {
    return navigator.onLine;
}

window.addEventListener('online', function () {
    showToast('success', 'Connection restored');
});

window.addEventListener('offline', function () {
    showToast('warning', 'You are offline');
});

// Prevent Double Submission
$('form').on('submit', function () {
    const submitBtn = $(this).find('button[type="submit"]');
    if (submitBtn.prop('disabled')) {
        return false;
    }
    showLoading(submitBtn);
});

// Global Error Handler
window.addEventListener('error', function (e) {
    console.error('Global error:', e.message);
});

// Performance Monitoring
window.addEventListener('load', function () {
    const perfData = window.performance.timing;
    const pageLoadTime = perfData.loadEventEnd - perfData.navigationStart;
    console.log(`Page load time: ${pageLoadTime}ms`);
});

// Export utility functions for use in other scripts
window.GymFitUtils = {
    formatCurrency,
    formatNumber,
    formatDate,
    formatDateTime,
    storage,
    debounce,
    copyToClipboard,
    printElement,
    downloadFile,
    showLoading,
    hideLoading,
    confirmAction
};
