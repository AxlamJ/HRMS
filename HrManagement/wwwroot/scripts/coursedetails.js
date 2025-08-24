
        // Enable Bootstrap tooltips
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl)
        });// Function to update the icon and text based on selected status// Function to update the icon and text based on selected status
    // Function to update the status for subtree items (post-cards)

    // Function to update the status for subtree items (post-cards)

    // Function to update the status for subtree items (post-cards)
    function updateSubTreeStatus(status, element) {
    const statusIconElement = element.closest('.dropdown').querySelector('.status-icon');
    const statusTextElement = element.closest('.dropdown').querySelector('.status-text');

    // Remove all existing icon and text color classes to prevent stacking
    statusIconElement.classList.remove('bi-check-circle-fill', 'bi-clipboard', 'bi-lock-fill', 'bi-clock-fill');
    statusIconElement.classList.remove('text-success', 'text-secondary', 'text-danger', 'text-primary');

    statusTextElement.classList.remove('text-success', 'text-secondary', 'text-danger', 'text-primary');

    // Add the appropriate icon, text, and text color based on selected status
    if (status === 'Published') {
        statusIconElement.classList.add('bi-check-circle-fill');
    statusTextElement.textContent = 'Published';
    statusIconElement.classList.add('text-success');  // Green color for "Published"
    statusTextElement.classList.add('text-success');  // Green color for "Published"
    } else if (status === 'Draft') {
        statusIconElement.classList.add('bi-clipboard');
    statusTextElement.textContent = 'Draft';
    statusIconElement.classList.add('text-secondary');  // Grey color for "Draft"
    statusTextElement.classList.add('text-secondary');  // Grey color for "Draft"
    } else if (status === 'Locked') {
        statusIconElement.classList.add('bi-lock-fill');
    statusTextElement.textContent = 'Locked';
    statusIconElement.classList.add('text-danger');  // Red color for "Locked"
    statusTextElement.classList.add('text-danger');  // Red color for "Locked"
    } else if (status === 'Drip') {
        statusIconElement.classList.add('bi-clock-fill');
    statusTextElement.textContent = 'Drip';
    statusIconElement.classList.add('text-primary');  // Blue color for "Drip"
    statusTextElement.classList.add('text-primary');  // Blue color for "Drip"
    }

  
}


    function updateStatusProduct(element, status) {
    // Get the status icon, text, and button elements
    const statusIconElement = document.getElementById('statusIcon');
    const statusTextElement = document.getElementById('statusText');
    const button = document.querySelector('.status-published');

    // Remove all existing icon, text color, and button color classes to prevent stacking
    statusIconElement.classList.remove('bi-check-circle-fill', 'bi-clipboard', 'bi-lock-fill', 'bi-clock-fill');
    statusIconElement.classList.remove('text-success', 'text-secondary', 'text-danger', 'text-primary');
    button.classList.remove('btn-success', 'btn-secondary', 'btn-danger', 'btn-primary'); // Remove existing button color classes

    statusTextElement.classList.remove('text-success', 'text-secondary', 'text-danger', 'text-primary');

    // Add the appropriate icon, text, button color, and text color based on selected status
    if (status === 'Published') {
        statusIconElement.classList.add('bi-check-circle-fill');
    statusTextElement.textContent = 'Published';
    statusIconElement.classList.add('text-success');  // Green color for "Published"
    statusTextElement.classList.add('text-success');  // Green color for "Published"
    button.classList.add('btn-success');  // Button background color for "Published"
    } else if (status === 'Draft') {
        statusIconElement.classList.add('bi-clipboard');
    statusTextElement.textContent = 'Draft';
    statusIconElement.classList.add('text-secondary');  // Grey color for "Draft"
    statusTextElement.classList.add('text-secondary');  // Grey color for "Draft"
    button.classList.add('btn-secondary');  // Button background color for "Draft"
    } else if (status === 'Locked') {
        statusIconElement.classList.add('bi-lock-fill');
    statusTextElement.textContent = 'Locked';
    statusIconElement.classList.add('text-danger');  // Red color for "Locked"
    statusTextElement.classList.add('text-danger');  // Red color for "Locked"
    button.classList.add('btn-danger');  // Button background color for "Locked"
    } else if (status === 'Drip') {
        statusIconElement.classList.add('bi-clock-fill');
    statusTextElement.textContent = 'Drip';
    statusIconElement.classList.add('text-primary');  // Blue color for "Drip"
    statusTextElement.classList.add('text-primary');  // Blue color for "Drip"
    button.classList.add('btn-primary');  // Button background color for "Drip"
    }

    // Close the dropdown after selection
    const dropdownInstance = bootstrap.Dropdown.getInstance(document.getElementById('statusDropdown'));
    dropdownInstance.hide();
}


