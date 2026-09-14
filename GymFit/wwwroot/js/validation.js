const validationHelper = {
    validateEmail: function (email) {
        const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return re.test(email);
    },

    validatePhone: function (phone) {
        const re = /^[\d\s\-\+\(\)]+$/;
        return re.test(phone) && phone.replace(/\D/g, '').length >= 10;
    },

    validatePassword: function (password) {
        // At least 6 characters, 1 uppercase, 1 lowercase, 1 number
        const re = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$/;
        return re.test(password);
    },

    validateForm: function (formId) {
        let isValid = true;
        const form = $(formId);

        // Clear previous errors
        form.find('.is-invalid').removeClass('is-invalid');
        form.find('.invalid-feedback').remove();

        // Validate required fields
        form.find('[required]').each(function () {
            const field = $(this);
            const value = field.val().trim();

            if (!value) {
                isValid = false;
                field.addClass('is-invalid');
                field.after('<div class="invalid-feedback">This field is required</div>');
            }
        });

        // Validate email fields
        form.find('[type="email"]').each(function () {
            const field = $(this);
            const value = field.val().trim();

            if (value && !validationHelper.validateEmail(value)) {
                isValid = false;
                field.addClass('is-invalid');
                field.after('<div class="invalid-feedback">Please enter a valid email</div>');
            }
        });

        // Validate phone fields
        form.find('[type="tel"]').each(function () {
            const field = $(this);
            const value = field.val().trim();

            if (value && !validationHelper.validatePhone(value)) {
                isValid = false;
                field.addClass('is-invalid');
                field.after('<div class="invalid-feedback">Please enter a valid phone number</div>');
            }
        });

        return isValid;
    },

    showFieldError: function (fieldId, message) {
        const field = $(fieldId);
        field.addClass('is-invalid');
        field.next('.invalid-feedback').remove();
        field.after(`<div class="invalid-feedback">${message}</div>`);
    },

    clearFieldError: function (fieldId) {
        const field = $(fieldId);
        field.removeClass('is-invalid');
        field.next('.invalid-feedback').remove();
    }
};

// Auto-clear errors on input
$(document).on('input', '.is-invalid', function () {
    $(this).removeClass('is-invalid');
    $(this).next('.invalid-feedback').remove();
});