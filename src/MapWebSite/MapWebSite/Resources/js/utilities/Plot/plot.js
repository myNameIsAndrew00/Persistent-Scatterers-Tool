/*!
 * Component: PlotDrawer
 * Handles plots drawing
 * */

const constants = {
    classes: {
        svgContainer: 'svg-container',
        svgContent: 'svg-content-responsive',
        grid: 'grid'
    }
}

/**
 * Use this class to draw a plot*/
export class PlotDrawer {

    /*
    *   Public methods
    */

    CreateContext(options) {
        /* options = {
                containerID: constants.ids.plot,
                length: {
                    oX: $(constants.ids.plot).width(),
                    oY: $(constants.ids.plot).height()
                },
                originAxesValue: {
                    oX: Math.round(oXLeft),
                    oY: Math.round(oYBottom) - 1
                },
                endAxesValue: {
                    oX: Math.round(oXRight),
                    oY: Math.round(oYTop)
                },
                labels: {
                    oX: constants.strings.plotOxLabel,
                    oY: constants.strings.plotOyLabel
                },
                plotType: 'points','bar','line',
                graphColor: 'white',
                margin: 20,
                viewBoxPadding: 0,
                points: [] {
                    data: [                    //an array of array of pairs {x: ,y:}
                        { X : 4, Y : 5}
                    ]
                    color: 'red',
                    hoverColor: 'red',
                    dataClickEvent: callback(d,i, {X:, Y:})
                }
            }
         * */
        var context = {
            ...options,
            plotInitialised: false,
            customAxis: {},
            svg: undefined,
            containerObject: undefined,
            yScale: undefined,
            xScale: undefined,
            plotData: undefined,
            linearRegressionPlot: undefined,
            polyRegressionPlot: undefined
        };
        if (context.viewBoxPadding === undefined) context.viewBoxPadding = 0;

        this.initContainer(context);

        return context;
    }

    /*
     * drawOptions : {
     *      erasePreviousPlot : false
     * }
     * */
    Draw(context, drawOptions) {

        if (drawOptions === undefined) drawOptions = {};

        if (context.plotData != undefined && drawOptions.erasePreviousPlot != false) context.plotData.selectAll("*").remove();

        if (context.plotInitialised === undefined || context.plotInitialised === false) {
            this.drawAxis(context);
            context.plotInitialised = true;
        }

        context.plotData = context.containerObject.append('g').attr('id', 'plot-data');

        for (var i = 0; i < context.points.length; i++)
            this.dataPlotDrawer(context, context.points[i])[context.plotType]();
    }


    /* 
     * drawOptions : {
     *      color: 'red',                   --mandatory
     *      identifier: identifier          --mandatory
     *      hideshow: true,
     *      value: 5,                       --mandatory
     *      width: 5                        --mandatory
     * }
     * */
    /**
     * Use this function to draw custom axis on current displayed plot
     * @param {any} context context required for drawing
     * @param {any} drawOptions options to draw the axe
     */
    DrawCustomAxis(context, drawOptions) {
        if (context.customAxis[drawOptions.identifier] !== undefined
            && context.customAxis[drawOptions.identifier] !== null
            && drawOptions.hideshow === true) {

            this.eraseSvg(context.customAxis[drawOptions.identifier]);
            context.customAxis[drawOptions.identifier] = null;
            return;
        }

        context.customAxis[drawOptions.identifier] =
            context.containerObject
                .append('rect')
                .attr('width', drawOptions.width)
                .attr('x', context.xScale(drawOptions.value))
                .attr('y', 0)
                .attr('height', context.length.oY)
                .style('fill', d3.color(drawOptions.color))
                .style('stroke', d3.color(drawOptions.color));

    }

    /*
     * regressionOptions : {
     *      type: 'linear', 'polynomial',
     *      hideshow: true,
     *      color: 'red',
     *      hoverColor: 'red',
     *      clickCallback: callback(coefficients, position)
     * }
     * */
    /**
     * Use this function to draw a plot regression on current displayed plot
     * @param {any} context context required for drawing
     * @param {any} regressionOptions an objects which contains data about the regression function which will be made
     */
    DrawPlotRegression(context, regressionOptions) {
        var self = this;

        function checkDrawingConditions(plotname) {

            if (context[plotname] !== undefined && context[plotname] !== null && regressionOptions.hideshow === true) {
                self.eraseSvg(context[plotname]);
                context[plotname] = null;
                return false;
            }

            context[plotname] = context.containerObject.append('g');

            return true;
        }

        //plotName represents the object (inside context) which will contain the plot
        //if the regression was never created, it will be created
        //if the regression exists and hideshow is true, the regression will be hide
        function drawRegression(plotName, data, addClickEvent) {
            if (!checkDrawingConditions(plotName))
                return;
            const strokeWidth = 2;

            var lineFunction = d3.line()
                .x(function (s) { return context.xScale(s[0]); })
                .y(function (s) { return context.yScale(s[1]); })


            addClickEvent(
                context[plotName]
                    .append("path")
                    .datum(data)
                    .attr('fill', 'none')
                    .attr('stroke', d3.color(regressionOptions.color))
                    .attr('stroke-width', strokeWidth)
                    .attr("d", lineFunction)
                    .style('transition', '0.3s')
                    .on('mouseover', function (d, i) {
                        d3.select(this)
                            .attr('stroke', d3.color(regressionOptions.hoverColor))
                            .attr('stroke-width', strokeWidth * 2)
                    })
                    .on('mouseout', function (d, i) {
                        d3.select(this)
                            .attr('stroke', d3.color(regressionOptions.color))
                            .attr('stroke-width', strokeWidth)
                    })
            );

        }

        switch (regressionOptions.type) {
            case 'linear':
                drawRegression(
                    'linearRegressionPlot',
                    d3.regressionLinear()
                        .x(d => d.X)
                        .y(d => d.Y)
                        .domain([context.originAxesValue.oX, context.endAxesValue.oX])(context.points[0].data),
                    function (builtContext) {
                        builtContext.on('click', function (d, i) {
                            regressionOptions.clickCallback(
                                [d.a, d.b],
                                { X: this.getBoundingClientRect().left, Y: this.getBoundingClientRect().top });
                        });
                    });
                break;
            case 'polynomial':
                drawRegression(
                    'polyRegressionPlot',
                    d3.regressionPoly()
                        .x(d => d.X)
                        .y(d => d.Y)
                        .order(3)(context.points[0].data),
                    function (builtContext) {
                        builtContext.on('click', function (d, i) {
                            regressionOptions.clickCallback(
                                d.coefficients,
                                { X: this.getBoundingClientRect().left, Y: this.getBoundingClientRect().top }); 
                        });
                    });
                break;
        }
    }

    /*
    *   Private methods 
    */

    eraseSvg(svg) {
        svg.selectAll("*").remove();
        svg.remove();
        svg = null;
    }

    dataPlotDrawer(context, displayedPoints) {

        const color = d3.color(displayedPoints.color === undefined ? context.graphColor : displayedPoints.color);
        const hoverColor = d3.color(displayedPoints.hoverColor === undefined ? context.graphColor : displayedPoints.hoverColor);

        function addInteraction(interactionContext) {
            interactionContext.on('mouseover', function (d, i) {
                d3.select(this)
                    .attr('fill', hoverColor)
                    .attr('stroke', hoverColor)
            })
                .on('mouseout', function (d, i) {
                    d3.select(this)
                        .attr('fill', color)
                        .attr('stroke', color)
                })
                .on('click', function (d, i) {

                    if (displayedPoints.dataClickEvent !== undefined) displayedPoints.dataClickEvent(
                        d,
                        i,
                        { X: this.getBoundingClientRect().left, Y: this.getBoundingClientRect().top });
                });
        }

        function bar() {
            addInteraction(
                context.plotData
                    .selectAll("bar")
                    .data(displayedPoints.data)
                    .enter()
                    .append('rect')
                    .attr('width', context.xScale.range().toString().replace(',', '.'))
                    .attr('x', function (s) {
                        return context.xScale(s.X);
                    })
                    .attr('y', (s) => context.yScale(s.Y))
                    .attr('height', (s) => context.length.oY - context.yScale(s.Y))
                    .attr('fill', color)
                    .attr('stroke', color)
            );
        }

        function line() {
            var lineFunction = d3.line()
                .x(function (s) { return context.xScale(s.X); })
                .y(function (s) { return context.yScale(s.Y); })


            context.plotData
                .append("path")
                .datum(displayedPoints.data)
                .style('fill', 'none')
                .style('stroke', color)
                .attr("d", lineFunction);

            points();
        }

        function points() {
            addInteraction(
                context.plotData
                    .selectAll(".dot")
                    .data(displayedPoints.data)
                    .enter().append("circle") // Uses the enter().append() method         
                    .attr("cx", function (d, i) { return context.xScale(d.X) })
                    .attr("cy", function (d) { return context.yScale(d.Y) })
                    .attr("r", 2)
                    .attr('fill', color)
                    .attr('stroke', color)
            );
        }

        return { bar, line, points };
    }



    drawAxis(context) {
        //oy axe
        context.containerObject.append('g')
            .call(d3.axisLeft(context.yScale));

        //ox axe
        context.containerObject.append('g')
            .attr('transform', `translate(0, ${context.length.oY})`)
            .call(d3.axisBottom(context.xScale));

        //grid
        context.containerObject.append('g')
            .attr('class', constants.classes.grid)
            .call(d3.axisLeft()
                .scale(context.yScale)
                .tickSize(-context.length.oX, 0, 0)
                .tickFormat(''))

        //labels
        context.svg.append('text')
            .attr('x', -(context.length.oY / 2) - context.margin)
            .attr('y', context.margin / 2.4)
            .attr('font-size', 10)
            .attr('transform', 'rotate(-90)')
            .attr('text-anchor', 'middle')
            .attr('fill', d3.color(context.graphColor))
            .text(context.labels.oY)

        context.svg.append('text')
            .attr('x', context.length.oX / 2 + context.margin)
            .attr('y', 40)
            .attr('font-size', 10)
            .attr('fill', d3.color(context.graphColor))
            .attr('text-anchor', 'middle')
            .attr('transform', `translate(0, ${context.length.oY}) `)
            .text(context.labels.oX)
    }

    initContainer(context) {
        $(context.containerID).empty();

        context.svg = d3.select(context.containerID)
            .append("div")
            .classed("svg-container", true)
            .append("svg")
            .attr("preserveAspectRatio", "xMinYMin meet")
            .attr("viewBox", `0 0 ${context.length.oX + context.margin + context.viewBoxPadding} ${context.length.oY + context.margin + context.viewBoxPadding}`)
            .classed("svg-content-responsive", true)

        context.containerObject = context.svg.append('g')
            .attr('transform', `translate(${context.margin}, 10 )`);

        context.yScale = d3.scaleLinear()
            .range([context.length.oY, 0])
            .domain([context.originAxesValue.oY, context.endAxesValue.oY]);
        context.xScale = d3.scaleLinear()
            .range([0, context.length.oX])
            .domain([context.originAxesValue.oX, context.endAxesValue.oX]);

        context.plotData = undefined;
    }

}

