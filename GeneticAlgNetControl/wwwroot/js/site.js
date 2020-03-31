// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.
$(window).on('load', () => {

    // Define the time interval in which refreshing takes place, in miliseconds.
    const _refreshInterval = 5000;

    // Check if the current page is "Dashboard".
    if ($('.current-page-dashboard').length !== 0) {

        // Check if there is a dropdown menu on the page.
        if ($('.dropdown-menu').length !== 0) {

            // Add a listener for licking inside the dropdown menu.
            $('.dropdown-menu').on('click', (event) => {
                // Stop the propagation.
                event.stopPropagation();
            });

        }

        // Check if there is a list group of items on the page.
        if ($('.item-group').length !== 0) {

            // Define a function which refreshes all items in the group.
            const refreshItems = (groupElement) => {
                // Get all of the items to update on the page.
                const itemsToUpdate = $(groupElement).find('.item-group-item').filter((index, element) => {
                    // Get the current status.
                    const status = $(element).find('.item-group-item-status').first().attr('title');
                    // Select the element if it needs updating.
                    return status === '' || status === 'Scheduled' || status === 'Initializing' || status === 'Ongoing' || status === 'Stopping';
                });
                // Check if any item needs updating.
                if (itemsToUpdate.length !== 0) {
                    // Go over all of the items that need updating and update them.
                    $(itemsToUpdate).each((index, element) => {
                        // Get the current status.
                        const status = $(element).find('.item-group-item-status').first().attr('title');
                        // Get the ID.
                        const id = $(element).find('.item-group-item-id').first().text();
                        // Retrieve the new data for the analysis with the mentioned ID.
                        const ajaxCall = $.ajax({
                            url: `${window.location.pathname}?handler=RefreshItem&id=${id}`,
                            dataType: 'json',
                            success: (json) => {
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
                                if (status !== json.statusTitle) {
                                    // Check the new status.
                                    if (json.statusTitle === 'Scheduled') {
                                        // Hide and show the corresponding buttons.
                                        $(element).find('.item-group-item-button-start').addClass('d-none');
                                        $(element).find('.item-group-item-button-stop').addClass('d-none');
                                        $(element).find('.item-group-item-button-save').addClass('d-none');
                                        $(element).find('.item-group-item-button-delete').removeClass('d-none');
                                    } else if (json.statusTitle === 'Ongoing') {
                                        // Hide and show the corresponding buttons.
                                        $(element).find('.item-group-item-button-start').addClass('d-none');
                                        $(element).find('.item-group-item-button-stop').removeClass('d-none');
                                        $(element).find('.item-group-item-button-save').addClass('d-none');
                                        $(element).find('.item-group-item-button-delete').addClass('d-none');
                                    } else if (json.statusTitle === 'Stopping') {
                                        // Hide and show the corresponding buttons.
                                        $(element).find('.item-group-item-button-start').addClass('d-none');
                                        $(element).find('.item-group-item-button-stop').addClass('d-none');
                                        $(element).find('.item-group-item-button-save').addClass('d-none');
                                        $(element).find('.item-group-item-button-delete').addClass('d-none');
                                    } else if (json.statusTitle === 'Stopped') {
                                        // Hide and show the corresponding buttons.
                                        $(element).find('.item-group-item-button-start').removeClass('d-none');
                                        $(element).find('.item-group-item-button-stop').addClass('d-none');
                                        $(element).find('.item-group-item-button-save').removeClass('d-none');
                                        $(element).find('.item-group-item-button-delete').removeClass('d-none');
                                    } else if (json.statusTitle === 'Completed') {
                                        // Hide and show the corresponding buttons.
                                        $(element).find('.item-group-item-button-start').addClass('d-none');
                                        $(element).find('.item-group-item-button-stop').addClass('d-none');
                                        $(element).find('.item-group-item-button-save').removeClass('d-none');
                                        $(element).find('.item-group-item-button-delete').removeClass('d-none');
                                    }
                                }
                            },
                            error: () => { }
                        });
                    });
                }
            };

            // Define a function which refreshes all statistics on page.
            const refreshStatistics = () => {
                // Go over each statistic element on the page.
                $('.item-group-statistic').each((index, element) => {
                    // Get the current statistic name.
                    const statisticName = $(element).find('.item-group-statistic-name').first().text();
                    // Retrieve the new data for the statistic with the mentioned name.
                    const ajaxCall = $.ajax({
                        url: `${window.location.pathname}?handler=RefreshStatistic&statisticName=${statisticName}`,
                        dataType: 'json',
                        success: (json) => {
                            // Update the corresponding field.
                            $(element).find('.item-group-statistic-count').text(json.statisticCount);
                        },
                        error: () => { }
                    });
                });
            };

            // Define a function which updates all items and statistics on page, as needed.
            const refresh = () => {
                // Add a refresh animation to the refresh button.
                $('.item-group-refresh').find('svg').first().addClass('fa-spin');
                // Go over each items group on the page and refresh its items.
                $('.item-group').each((index, element) => refreshItems(element));
                // Refresh all of the statistics on the page.
                refreshStatistics();
                // Remove the refresh animation from the refresh button.
                $('.item-group-refresh').find('svg').removeClass('fa-spin');
            };

            // Define a function which gets all of the selected items and creates a JSON string array with their IDs.
            const getSelectedItems = (groupElement) => {
                // Get all of the items.
                const items = $(groupElement).find('.item-group-item');
                // Remove the active class from all list items.
                $(items).removeClass('active');
                // Go over all of the checked elements and get the corresponding list group items.
                const selectedItems = $(items).find('input[type="checkbox"]:checked').closest('.item-group-item');
                // Go over each of the selected items and mark them as active.
                $(selectedItems).addClass('active');
                // Check if there are no elements selected.
                if (selectedItems.length === 0) {
                    // Disable the group buttons.
                    $(groupElement).find('.item-group-button').prop('disabled', true);
                    // Unmark the checkbox as indeterminate.
                    $(groupElement).find('.item-group-select').prop('indeterminate', false);
                    // Uncheck the checkbox.
                    $(groupElement).find('.item-group-select').prop('checked', false);
                } else {
                    // Get an array containing the statuses of the selected elements.
                    const statuses = $(selectedItems).find('.item-group-item-status').map((index, element) => $(element).attr('title')).toArray();
                    // Check if the 'Start' button should be enabled (if all / any of the selected elements has the status 'Stopped').
                    if (statuses.length !== 0 && statuses.every((item) => item === 'Stopped')) {
                        // Enable the 'Start' button.
                        $(groupElement).find('.item-group-button-start').prop('disabled', false);
                    }
                    // Check if the 'Stop' button should be enabled (if all / any of the selected elements has the status 'Ongoing').
                    if (statuses.length !== 0 && statuses.every((item) => item === 'Ongoing')) {
                        // Enable the 'Stop' button.
                        $(groupElement).find('.item-group-button-stop').prop('disabled', false);
                    }
                    // Check if the 'Save' button should be enabled (if all / any of the selected elements has the status 'Stopped' or 'Completed').
                    if (statuses.length !== 0 && statuses.every((item) => item === 'Stopped' || item === 'Completed')) {
                        // Enable the 'Save' button.
                        $(groupElement).find('.item-group-button-save').prop('disabled', false);
                    }
                    // Check if the 'Delete' button should be enabled (if all / any of the selected elements has the status 'Scheduled', 'Stopped' or 'Completed').
                    if (statuses.length !== 0 && statuses.every((item) => item === 'Scheduled' || item === 'Stopped' || item === 'Completed')) {
                        // Enable the 'Delete' button.
                        $(groupElement).find('.item-group-button-delete').prop('disabled', false);
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
                // Get the current group element.
                const groupElement = $(event.target).closest('.item-group');
                // Update the selected items.
                getSelectedItems(groupElement);
            });

            // Add a listener for the "select" checkbox.
            $('.item-group-select').on('change', (event) => {
                // Get the current group element.
                const groupElement = $(event.target).closest('.item-group');
                // Check if the checkbox is currently checked.
                if ($(event.target).prop('checked')) {
                    // Check all of the checkboxes in the group.
                    $(groupElement).find('input[type="checkbox"]:not(:checked)').prop('checked', true);
                } else {
                    // Uncheck all of the checkboxes in the group.
                    $(groupElement).find('input[type="checkbox"]:checked').prop('checked', false);
                }
                // Update the selected items.
                getSelectedItems(groupElement);
            });

            // Add a listener for clicking the "refresh" button.
            $('.item-group-refresh').on('click', (event) => {
                // Update everything on page.
                refresh();
                // Don't follow any link.
                event.preventDefault();
            });

            // Execute the function on page load.
            (() => {
                // Refresh everything.
                refresh();
                // Go over each of the item groups on the page.
                $('.item-group').each((index, element) => {
                    // Get the selected items.
                    getSelectedItems(element);
                    // Get an array containing the statuses of all items.
                    const statuses = $(element).find('.item-group-item-status').map((index, element) => $(element).attr('title')).toArray();
                    // Check if the page will need refreshing.
                    if (statuses.some((item) => item === '' || item === 'Scheduled' || item === 'Initializing' || item === 'Ongoing' || item === 'Stopping')) {
                        // Refresh everything every few seconds.
                        setInterval(() => refresh(), _refreshInterval);
                    }
                });
            })();

        }

    }

    // Check if the current page is "Create".
    if ($('.current-page-create').length !== 0) {

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

            // Execute the function on page load.
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

    }

    // Check if the current page is "Details".
    if ($('.current-page-details').length !== 0) {

        // Define a function to refresh the details of the current item.
        const refresh = (element) => {
            // Get the ID of the item.
            const id = $(element).find('.item-details-id').first().text();
            // Get the current status of the item.
            const status = $(element).find('.item-details-status').first().attr('title');
            // Retrieve the new data for the item with the mentioned ID.
            const ajaxCall = $.ajax({
                url: `${window.location.pathname}?handler=Refresh&id=${id}`,
                dataType: 'json',
                success: (json) => {
                    // Check if the status has changed.
                    if (status !== json.statusTitle) {
                        // Reload the page.
                        location.reload(true);
                    }
                    // Update the fields.
                    $(element).find('.item-details-current-iteration').attr('title', json.currentIterationTitle);
                    $(element).find('.item-details-current-iteration').text(json.currentIterationText);
                    $(element).find('.item-details-current-iteration-without-improvement').attr('title', json.currentIterationWithoutImprovementTitle);
                    $(element).find('.item-details-current-iteration-without-improvement').text(json.currentIterationWithoutImprovementText);
                    $(element).find('.item-details-date-time-started').attr('title', json.dateTimeStartedTitle);
                    $(element).find('.item-details-date-time-started').text(json.dateTimeStartedText);
                    $(element).find('.item-details-time-span').attr('title', json.timeSpanTitle);
                    $(element).find('.item-details-time-span').text(json.timeSpanText);
                    $(element).find('.item-details-date-time-ended').attr('title', json.dateTimeEndedTitle);
                    $(element).find('.item-details-date-time-ended').text(json.dateTimeEndedText);
                },
                error: () => { }
            });
        };

        // Define a function to style a table on the page with DataTables.
        const paintDataTable = (element) => {
            // Make it a datatable.
            $(element).DataTable();
        };

        // Define a function to paint a chart element on the page.
        const paintChart = (element) => {
            // Get the type, data and the canvas element.
            const chartType = $(element).data('chart-type');
            const chartData = JSON.parse($(element).find('.chart-js-data').first().text());
            const chartCanvas = $(element).find('.chart-js-canvas').first();
            // Define the chart.
            const chart = new Chart(chartCanvas, {
                type: chartType,
                data: {
                    labels: [...Array(chartData.AverageFitness.length).keys()],
                    datasets: [{
                        label: 'Average fitness',
                        data: chartData.AverageFitness,
                        backgroundColor: 'rgba(54, 162, 235, 1)',
                        borderColor: 'rgba(54, 162, 235, 1)',
                        borderWidth: 1,
                        fill: false
                    },
                    {
                        label: 'Best fitness',
                        data: chartData.BestFitness,
                        backgroundColor: 'rgba(75, 192, 192, 1)',
                        borderColor: 'rgba(75, 192, 192, 1)',
                        borderWidth: 1,
                        fill: false
                    }]
                },
                options: {
                    title: {
                        display: true,
                        text: 'Best and average fitness over the iterations'
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
                    },
                    animation: {
                        duration: 0
                    },
                    hover: {
                        animationDuration: 0
                    },
                    responsiveAnimationDuration: 0
                }
            });
        };

        // Execute the function on page load.
        (() => {
            // Get the current status.
            const currentStatus = $('.item-details-status').attr('title');
            // Go over each of the charts and paint them.
            $('.chart-js-chart').each((index, element) => paintChart(element));
            // Go over each of the tables and format them.
            $('table.table-datatable').each((index, element) => paintDataTable(element));
            // Check if the page needs to be refreshed.
            if (currentStatus === '' || currentStatus === 'Scheduled' || currentStatus === 'Initializing' || currentStatus === 'Ongoing' || currentStatus === 'Stopping') {
                // Repeat the function every few seconds.
                setInterval(() => {
                    // Go over all elements in the page.
                    $('.item-details').each((index, element) => refresh(element));
                }, _refreshInterval);
            }
        })();

    }

});