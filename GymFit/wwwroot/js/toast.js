function showToast(type, message, duration = 3000) {
    // Remove any existing toasts
    $('.custom-toast').remove();

    // Determine icon and color based on type
    let icon, bgColor;
    switch (type) {
        case 'success':
            icon = 'fa-check-circle';
            bgColor = '#28a745';
            break;
        case 'error':
            icon = 'fa-exclamation-circle';
            bgColor = '#dc3545';
            break;
        case 'warning':
            icon = 'fa-exclamation-triangle';
            bgColor = '#ffc107';
            break;
        case 'info':
            icon = 'fa-info-circle';
            bgColor = '#17a2b8';
            break;
        default:
            icon = 'fa-bell';
            bgColor = '#6c757d';
    }

    // Create toast HTML
    const toastHtml = `
        <div class="custom-toast" style="
            position: fixed;
            top: 20px;
            right: 20px;
            background: ${bgColor};
            color: white;
            padding: 1rem 1.5rem;
            border-radius: 10px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.3);
            z-index: 9999;
            display: flex;
            align-items: center;
            gap: 10px;
            animation: slideIn 0.3s ease-out;
            min-width: 300px;
            max-width: 500px;
        ">
            <i class="fas ${icon}" style="font-size: 1.5rem;"></i>
            <span style="flex: 1;">${message}</span>
            <button onclick="$(this).parent().remove()" style="
                background: none;
                border: none;
                color: white;
                font-size: 1.2rem;
                cursor: pointer;
                padding: 0;
                margin-left: 10px;
            ">×</button>
        </div>
    `;

    // Add CSS animation
    if (!$('#toast-animation-style').length) {
        $('head').append(`
            <style id="toast-animation-style">
                @keyframes slideIn {
                    from {
                        transform: translateX(400px);
                        opacity: 0;
                    }
                    to {
                        transform: translateX(0);
                        opacity: 1;
                    }
                }
                @keyframes slideOut {
                    from {
                        transform: translateX(0);
                        opacity: 1;
                    }
                    to {
                        transform: translateX(400px);
                        opacity: 0;
                    }
                }
            </style>
        `);
    }

    // Append toast to body
    $('body').append(toastHtml);

    // Auto remove after duration
    setTimeout(() => {
        $('.custom-toast').css('animation', 'slideOut 0.3s ease-in');
        setTimeout(() => $('.custom-toast').remove(), 300);
    }, duration);
}