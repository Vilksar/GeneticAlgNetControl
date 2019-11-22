// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.
$(window).on('load', () => {

    // Define the time interval in which refreshing takes place, in miliseconds.
    const _refreshInterval = 5000;

    // Check if there is a dropdown menu form on the page.
    if ($('.dropdown-menu-form').length !== 0) {
        // Add a listener for licking inside the dropdown menu.
        $('.dropdown-menu.dropdown-menu-form').on('click', (event) => {
            // Stop the propagation.
            event.stopPropagation();
        });
    }

    // Check if there is a list group of items on the page.
    if ($('.item-group').length !== 0) {
        // Define a function which gets all of the selected items and creates a JSON string array with their IDs.
        const updateSelectedItems = (groupElement) => {
            // Get all of the list group items.
            const items = $(groupElement).find('.item-group-item');
            // Remove the active class from all list items.
            $(items).removeClass('active');
            // Go over all of the checked elements and get the corresponding list group items.
            const selectedItems = $(groupElement).find('input[type="checkbox"]:checked').closest('.item-group-item');
            // Go over each of the selected items and mark them as active.
            $(selectedItems).addClass('active');
            // Check if there are no elements selected.
            if (selectedItems.length === 0) {
                // Disable the group buttons.
                $('.item-group-button').prop('disabled', true);
                // Unmark the checkbox as indeterminate.
                $(groupElement).find('.item-group-select').prop('indeterminate', false);
                // Uncheck the checkbox.
                $(groupElement).find('.item-group-select').prop('checked', false);
            } else {
                // Get an array containing the statuses of the selected elements.
                const statuses = $(selectedItems).find('.item-group-item-status').map((index, element) => $(element).text()).toArray();
                // Check if the 'Start' button should be enabled (if all / any of the selected elements has the status 'Stopped').
                if (statuses.length !== 0 && statuses.every((item) => item === 'Stopped')) {
                    // Enable the 'Start' button.
                    $('.item-group-button-start').prop('disabled', false);
                }
                // Check if the 'Stop' button should be enabled (if all / any of the selected elements has the status 'Ongoing').
                if (statuses.length !== 0 && statuses.every((item) => item === 'Ongoing')) {
                    // Enable the 'Stop' button.
                    $('.item-group-button-stop').prop('disabled', false);
                }
                // Check if the 'Save' button should be enabled (if all / any of the selected elements has the status 'Stopped' or 'Completed').
                if (statuses.length !== 0 && statuses.every((item) => item === 'Stopped' || item === 'Completed')) {
                    // Enable the 'Save' button.
                    $('.item-group-button-save').prop('disabled', false);
                }
                // Check if the 'Delete' button should be enabled (if all / any of the selected elements has the status 'Scheduled', 'Stopped' or 'Completed').
                if (statuses.length !== 0 && statuses.every((item) => item === 'Scheduled' || item === 'Stopped' || item === 'Completed')) {
                    // Enable the 'Delete' button.
                    $('.item-group-button-delete').prop('disabled', false);
                }
                // Check if not all elements are selected.
                if (selectedItems.length !== items.length) {
                    // Mark the checkbox as indeterminate.
                    $(groupElement).find('.item-group-select').prop('indeterminate', true);
                } else {
                    // Unmark the checkbox as indeterminate.
                    $(groupElement).find('.item-group-select').prop('indeterminate', false);
                    // Check the checkbox.
                    $(groupElement).find('.item-group-select').prop('checked', true);
                }
            }
        };
        // Add a listener for when a checkbox gets checked or unchecked.
        $('.item-group').on('change', 'input[type="checkbox"]', (event) => {
            // Get the current list group.
            const groupElement = $(event.target).closest('.item-group');
            // Update the selected items.
            updateSelectedItems(groupElement);
        });
        // Add a listener for the select checkbox.
        $('.item-group-select').on('change', (event) => {
            // Get the current list group.
            const groupElement = $(event.target).closest('.item-group');
            // Check if the checkbox is currently checked.
            if ($(event.target).prop('checked')) {
                // Check all of the checkboxes on the page.
                $(groupElement).find('input[type="checkbox"]:not(:checked)').prop('checked', true);
            } else {
                // Uncheck all of the checkboxes on the page.
                $(groupElement).find('input[type="checkbox"]:checked').prop('checked', false);
            }
            // Update the selected items.
            updateSelectedItems(groupElement);
        });
        // On page load, parse the input and check the group items.
        (() => {
            // Go over all of the groups.
            $('.item-group').each((index, element) => {
                // Update the selected items.
                updateSelectedItems(element);
            });
        })();
    }

    // Check if there is a file protein group on the page.
    if ($('.file-group').length !== 0) {
        // Define a function which updates the data to be submitted.
        const updateText = (groupElement) => {
            // Check if the text is empty.
            if (!$.trim($(groupElement).find('.file-group-text').first().val())) {
                // Update the value of the count.
                $(groupElement).find('.file-group-count').first().text(0);
                // Update the data to be submitted.
                $(groupElement).find('.file-group-input').first().val(JSON.stringify([]));
                // Return from the function.
                return;
            }
            // Get the in-line separator.
            const inlineSeparator = $(groupElement).find('.file-group-in-line-separator').first().val();
            // Get the line separator.
            const lineSeparator = $(groupElement).find('.file-group-line-separator').first().val();
            // Get the type of the file group.
            const type = $(groupElement).data('type');
            // Check if we have nodes.
            if (type === 'nodes') {
                // Split the text into different lines.
                const rows = $(groupElement).find('.file-group-text').first().val().split(new RegExp(lineSeparator)).filter((element) => {
                    // Select only the non empty elements.
                    return $.trim(element);
                });
                // Update the value of the count.
                $(groupElement).find('.file-group-count').first().text(rows.length);
                // Update the data to be submitted.
                $(groupElement).find('.file-group-input').first().val(JSON.stringify(rows));
            }
            // Check if we have interactions.
            else if (type === 'edges') {
                // Split the text into different lines.
                const rows = $(groupElement).find('.file-group-text').first().val().split(new RegExp(lineSeparator)).filter((element) => {
                    // Split the row into its composing items.
                    var row = element.split(new RegExp(inlineSeparator)).filter((el) => {
                        // Select only the non-empty items.
                        return el !== '';
                    });
                    // Select only elements with more than two items.
                    return row.length === 2;
                });
                // Go over each row.
                const items = $.map(rows, (element, index) => {
                    // Split the row into its composing items.
                    var row = element.split(new RegExp(inlineSeparator));
                    // Check if we don't have both source and target nodes.
                    if (!row[0] || !row[1]) {
                        // Don't return anything.
                        return;
                    }
                    // Split the element into an array of items.
                    return { 'SourceNode': row[0], 'TargetNode': row[1] };
                });
                // Update the value of the count.
                $(groupElement).find('.file-group-count').first().text(rows.length);
                // Update the data to be submitted.
                $(groupElement).find('.file-group-input').first().val(JSON.stringify(items));
            }
        };
        // Add a listener for typing into the text area.
        $('.file-group-text').on('keyup', (event) => {
            // Get the actual group which was clicked.
            const groupElement = $(event.target).closest('.file-group');
            // Update the selected items.
            updateText(groupElement);
        });
        // Add a listener for changing one of the separators.
        $('.file-group').on('change', '.file-group-separator', (event) => {
            // Get the actual group which was clicked.
            const groupElement = $(event.target).closest('.file-group');
            // Update the selected items.
            updateText(groupElement);
        });
        // Add a listener for if the upload button was clicked.
        $('.file-group-file-upload').on('change', (event) => {
            // Get the current file.
            const file = event.target.files[0];
            // Set the filename in the label.
            $(event.target).siblings('.file-group-file-label').html(file.name);
            // Define the file reader and the variable for storing its content.
            let fileReader = new FileReader();
            // Define what happens when we read the file.
            fileReader.onload = (e) => {
                // Write the content of the file to the text area.
                $(event.target).closest('.file-group').find('.file-group-text').first().val(e.target.result);
                // Get the actual group which was clicked.
                const groupElement = $(event.target).closest('.file-group');
                // Update the selected items.
                updateText(groupElement);
            };
            // Read the file.
            fileReader.readAsText(file);
        });
        // On page load, parse the input and add the group items.
        (() => {
            // Go over all of the groups.
            $('.file-group').each((index, groupElement) => {
                // Get the type of the file group.
                const type = $(groupElement).data('type');
                // Define a variable for the input data.
                let data = undefined;
                // Try to parse the input data.
                try {
                    // Get the input data.
                    data = JSON.parse($(groupElement).find('.file-group-input').first().val());
                }
                catch (error) {
                    // Return from the function.
                    return;
                }
                // Check if there isn't any data.
                if (typeof data === 'undefined') {
                    // Return from the function.
                    return;
                }
                // Check if we have a proper array.
                if (!Array.isArray(data)) {
                    // Return from the function.
                    return;
                }
                // Check if we have nodes.
                if (type === 'nodes') {
                    // Go over all of the elements.
                    data = data.filter((element) => {
                        // Keep only the ones which are of the proper type.
                        return typeof element === 'string';
                    });
                    // Add the elements to the text.
                    $(groupElement).find('.file-group-text').first().val(data.join('\n'));
                }
                // Check if we have edges.
                if (type === 'edges') {
                    // Go over all of the elements.
                    data = data.filter((element) => {
                        // Keep only the ones which are of the proper type.
                        return element['SourceNode'] && typeof element['SourceNode'] === 'string' && element['TargetNode'] && typeof element['TargetNode'] === 'string';
                    });
                    // Go over all of the elements.
                    data = $.map(data, (element, index) => {
                        // Check if we have more than one of each.
                        if (!element['SourceNode'] || !element['TargetNode']) {
                            // Return an undefined value.
                            return undefined;
                        } else {
                            // Return a simplified object.
                            return `${element['SourceNode']};${element['TargetNode']}`;
                        }
                    });
                    // Add the elements to the text.
                    $(groupElement).find('.file-group-text').first().val(data.join('\n'));
                }
                // Update the selected items.
                updateText(groupElement);
            });
        })();
    }

    // Check if there is any item group item on page.
    if ($('.item-group-item').length !== 0) {
        // Define a function which updates an item.
        const updateItem = (element) => {
            // Get the current status.
            const status = $(element).find('.item-group-item-status').first().text();
            // Get the ID.
            const id = $(element).find('.item-group-item-id').first().text();
            // Retrieve the new data for the algorithm with the mentioned ID.
            const json = $.ajax({
                url: `${window.location.pathname}?handler=RefreshAlgorithm&id=${id}`,
                async: false,
                dataType: 'json'
            }).responseJSON;
            // Update the corresponding fields.
            $(element).find('.item-group-item-status').attr('title', json.statusTitle);
            $(element).find('.item-group-item-status').text(json.statusText);
            $(element).find('.item-group-item-time-span').attr('title', json.timeSpanTitle);
            $(element).find('.item-group-item-time-span').text(json.timeSpanText);
            $(element).find('.item-group-item-progress-iterations').attr('title', json.progressIterationsTitle);
            $(element).find('.item-group-item-progress-iterations').text(json.progressIterationsText);
            $(element).find('.item-group-item-progress-iterations-without-improvement').attr('title', json.progressIterationsWithoutImprovementTitle);
            $(element).find('.item-group-item-progress-iterations-without-improvement').text(json.progressIterationsWithoutImprovementText);
            // Check if the status has been updated.
            if (status !== json.statusText) {
                // Check the new status.
                if (json.statusText === 'Scheduled') {
                    // Hide and show the corresponding buttons.
                    $(element).find('.item-group-item-button-start').addClass('d-none');
                    $(element).find('.item-group-item-button-stop').addClass('d-none');
                    $(element).find('.item-group-item-button-save').addClass('d-none');
                    $(element).find('.item-group-item-button-delete').removeClass('d-none');
                } else if (json.statusText === 'Ongoing') {
                    // Hide and show the corresponding buttons.
                    $(element).find('.item-group-item-button-start').addClass('d-none');
                    $(element).find('.item-group-item-button-stop').removeClass('d-none');
                    $(element).find('.item-group-item-button-save').addClass('d-none');
                    $(element).find('.item-group-item-button-delete').addClass('d-none');
                } else if (json.statusText === 'ScheduledToStop') {
                    // Hide and show the corresponding buttons.
                    $(element).find('.item-group-item-button-start').addClass('d-none');
                    $(element).find('.item-group-item-button-stop').addClass('d-none');
                    $(element).find('.item-group-item-button-save').addClass('d-none');
                    $(element).find('.item-group-item-button-delete').addClass('d-none');
                } else if (json.statusText === 'Stopped') {
                    // Hide and show the corresponding buttons.
                    $(element).find('.item-group-item-button-start').removeClass('d-none');
                    $(element).find('.item-group-item-button-stop').addClass('d-none');
                    $(element).find('.item-group-item-button-save').removeClass('d-none');
                    $(element).find('.item-group-item-button-delete').removeClass('d-none');
                } else if (json.statusText === 'Completed') {
                    // Hide and show the corresponding buttons.
                    $(element).find('.item-group-item-button-start').addClass('d-none');
                    $(element).find('.item-group-item-button-stop').addClass('d-none');
                    $(element).find('.item-group-item-button-save').removeClass('d-none');
                    $(element).find('.item-group-item-button-delete').removeClass('d-none');
                }
            }
        };
        // Define a function which updates all statistics on page.
        const updateAllStatistics = () => {
            // Go over each statistic element on the page.
            $('.item-group-statistic').each((index, element) => {
                // Get the current statistic name.
                const statisticName = $(element).find('.item-group-statistic-name').first().text();
                // Retrieve the new data for the statistic with the mentioned name.
                const json = $.ajax({
                    url: `${window.location.pathname}?handler=RefreshStatistic&statisticName=${statisticName}`,
                    async: false,
                    dataType: 'json'
                }).responseJSON;
                // Update the corresponding field.
                $(element).find('.item-group-statistic-count').text(json.statisticCount);
            });
        };
        // Define a function which updates all items and statistics on page, as needed.
        const updateAll = () => {
            // Add a refresh animation to the refresh button.
            $('.item-group-refresh').find('svg').first().addClass('fa-spin');
            // Get all of the items to update on page.
            const itemsToUpdate = $('.item-group-item').filter((index, element) => {
                // Get the current status.
                const status = $(element).find('.item-group-item-status').first().text();
                // Select the element if it needs updating.
                return status === '' || status === 'PreparingToStart' || status === 'Scheduled' || status === 'Ongoing' || status === 'ScheduledToStop';
            });
            // Check if any item needs updating.
            if (itemsToUpdate.length !== 0) {
                // Go over all of the items that do.
                $(itemsToUpdate).each((index, element) => updateItem(element));
                // Update all statistics on page.
                updateAllStatistics();
            }
            // Remove the refresh animation from the refresh button.
            $('.item-group-refresh').find('svg').removeClass('fa-spin');
        };
        // Add a listener for clicking the "refresh" button.
        $('.item-group-refresh').on('click', (event) => {
            // Update everything on page.
            updateAll();
            // Don't follow any link.
            event.preventDefault();
        });
        // Update everything on page load.
        updateAll();
        // Repeat the function every few seconds.
        setInterval(function () {
            // Update everything.
            updateAll();
        }, _refreshInterval);
    }

    // Check if there is any algorithm whose details to refresh on page.
    if ($('.algorithm-details-status').length !== 0) {
        // Get the current status.
        const currentStatus = $('.algorithm-details-status').val();
        // Check if the page needs updating.
        if (currentStatus === 'Scheduled' || currentStatus === 'Ongoing' || currentStatus === 'ScheduledToStop') {
            // Repeat the function every few seconds.
            setInterval(function () {
                // Get the ID of the algorithm.
                let id = $('.algorithm-details-id').val();
                // Retrieve the new data for the algorithm with the mentioned ID.
                let json = $.ajax({
                    url: `/Details?handler=Refresh&id=${id}`,
                    async: false,
                    dataType: 'json'
                }).responseJSON;
                // Check if the algorithm has ended.
                if (json.status === 'Stopped' || json.status === 'Completed') {
                    // Reload the page.
                    location.reload(true);
                }
                // Update the corresponding fields.
                $('.algorithm-details-current-iteration').val(json.currentIteration);
                $('.algorithm-details-current-iteration-without-improvement').val(json.currentIterationWithoutImprovement);
            }, _refreshInterval);
        }
    }

    // Check if there is any details modal on the page.
    if ($('.details-modal').length !== 0) {
        // Add a listener for modal opening.
        $('.details-modal').on('show.bs.modal', (event) => {
            // Get the group that trigerred the modal.
            const group = $(event.relatedTarget).closest('.modal-group');
            // Update the modal's content.
            $('.details-modal').first().find('.modal-title').html($(group.find('.modal-group-title').first().html()));
            $('.details-modal').first().find('.modal-body').html($(group.find('.modal-group-body').first().html()));
        });
    }

    // Check if there is any chart on the page.
    if ($('.chart-js-chart').length !== 0) {
        // Go over each of the charts.
        $('.chart-js-chart').each((index, element) => {
            // Get the type, data and the canvas element.
            const chartType = $(element).data('type');
            const chartData = JSON.parse($(element).find('.chart-js-data').first().text());
            const chartCanvas = $(element).find('.chart-js-canvas').first();
            // Set the chart based on the type.
            if (chartType === 'dashboard') {
                // Define the chart.
                const chart = new Chart(chartCanvas, {
                    type: 'bar',
                    data: {
                        labels: ['Algorithms'],
                        datasets: [{
                            label: 'Scheduled',
                            data: chartData.Scheduled,
                            backgroundColor: 'rgba(255, 206, 86, 0.2)',
                            borderColor: 'rgba(255, 206, 86, 1)',
                            borderWidth: 1
                        },
                        {
                            label: 'Ongoing',
                            data: chartData.Ongoing,
                            backgroundColor: 'rgba(54, 162, 235, 0.2)',
                            borderColor: 'rgba(54, 162, 235, 1)',
                            borderWidth: 1
                        },
                        {
                            label: 'Stopped',
                            data: chartData.Stopped,
                            backgroundColor: 'rgba(255, 99, 132, 0.2)',
                            borderColor: 'rgba(255, 99, 132, 1)',
                            borderWidth: 1
                        },
                        {
                            label: 'Completed',
                            data: chartData.Completed,
                            backgroundColor: 'rgba(75, 192, 192, 0.2)',
                            borderColor: 'rgba(75, 192, 192, 1)',
                            borderWidth: 1
                        }]
                    },
                    options: {
                        title: {
                            display: true,
                            text: 'Current algorithms'
                        },
                        scales: {
                            yAxes: [{
                                ticks: {
                                    beginAtZero: true
                                }
                            }]
                        }
                    }
                });
            } else if (chartType === 'details') {
                // Define the chart.
                const chart = new Chart(chartCanvas, {
                    type: 'line',
                    data: {
                        labels: [...Array(chartData.AverageFitness.length).keys()],
                        datasets: [{
                            label: 'Average fitness',
                            data: chartData.AverageFitness,
                            backgroundColor: 'rgba(54, 162, 235, 0)',
                            borderColor: 'rgba(54, 162, 235, 1)',
                            borderWidth: 1
                        },
                        {
                            label: 'Best fitness',
                            data: chartData.BestFitness,
                            backgroundColor: 'rgba(75, 192, 192, 0)',
                            borderColor: 'rgba(75, 192, 192, 1)',
                            borderWidth: 1
                        }]
                    },
                    options: {
                        title: {
                            display: true,
                            text: 'Progress'
                        },
                        scales: {
                            yAxes: [{
                                ticks: {
                                    min: 0,
                                    max: 100
                                }
                            }]
                        },
                        elements: {
                            line: {
                                tension: 0
                            }
                        }
                    }
                });
            }
        });
    }
});