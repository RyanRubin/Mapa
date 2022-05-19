class App {
    map;
    tileLayer;
    bingMapsSource;
    offlineMapsSource;
    vectorLayer;
    vectorSource;
    view;
    isRendered;

    initMap() {
        // window.addEventListener('contextmenu', e => e.preventDefault());

        this.bingMapsSource = new ol.source.BingMaps({
            key: 'AkGBls_Q10K9PW0jYdc1KuSPiF5qOXXICG3D9F2cT5QWLzdJsVYwgl8JYpnZg8sE',
            imagerySet: 'Aerial',
            maxZoom: 19, // https://openlayers.org/en/latest/examples/bing-maps.html
            tileLoadFunction: (imageTile, src) => {
                imageTile.getImage().src = src;
                // console.log(imageTile.tileCoord);
            }
        });

        this.offlineMapsSource = new ol.source.OSM({
            tileLoadFunction: (imageTile, src) => {
                imageTile.getImage().src = src;
            }
        });

        this.tileLayer = new ol.layer.Tile({
            source: this.bingMapsSource
        });

        this.vectorSource = new ol.source.Vector();

        this.vectorLayer = new ol.layer.Vector({
            source: this.vectorSource
        });

        this.view = new ol.View();

        this.map = new ol.Map({
            target: 'map',
            layers: [this.tileLayer, this.vectorLayer],
            view: this.view
        });

        this.map.once('rendercomplete', () => {
            this.isRendered = true;
        });
    }

    addIcon(lat, lon, src) {
        const iconFeature = new ol.Feature({
            geometry: new ol.geom.Point(ol.proj.fromLonLat([lon, lat]))
        });
        iconFeature.setStyle(new ol.style.Style({
            image: new ol.style.Icon({
                anchor: [0.5, 46],
                anchorXUnits: 'fraction',
                anchorYUnits: 'pixels',
                src: src
            })
        }));

        this.vectorSource.addFeature(iconFeature);
    }

    addCircle(lat, lon, color) {
        const circleFeature = new ol.Feature({
            geometry: new ol.geom.Point(ol.proj.fromLonLat([lon, lat]))
        });
        circleFeature.setStyle(new ol.style.Style({
            image: new ol.style.Circle({
                radius: 8,
                fill: new ol.style.Fill({
                    color: color
                })
            })
        }));

        this.vectorSource.addFeature(circleFeature);
    }

    addLine(lat1, lon1, lat2, lon2, color) {
        const lineFeature = new ol.Feature({
            geometry: new ol.geom.LineString([ol.proj.fromLonLat([lon1, lat1]), ol.proj.fromLonLat([lon2, lat2])])
        });
        lineFeature.setStyle(new ol.style.Style({
            stroke: new ol.style.Stroke({
                color: color,
                width: 1.25,
                lineDash: [5, 5]
            })
        }));

        this.vectorSource.addFeature(lineFeature);
    }

    addPopover(lat, lon, content) {
        const add = () => {
            const popoverEl = document.createElement('div');
            document.body.append(popoverEl);

            const overlay = new ol.Overlay({
                element: popoverEl,
                position: ol.proj.fromLonLat([lon, lat])
            });
            this.map.addOverlay(overlay);

            const popover = new bootstrap.Popover(popoverEl, {
                container: popoverEl,
                placement: 'top',
                animation: false,
                html: true,
                content: content,
                trigger: 'manual'
            });
            popover.show();
        };
        if (this.isRendered) {
            add();
        } else {
            this.map.once('rendercomplete', () => {
                add();
            });
        }
    }

    fitToFeaturesExtent() {
        const vectorExtent = this.vectorSource.getExtent();
        this.view.fit(vectorExtent, {
            padding: [25, 25, 25, 25],
            maxZoom: 19
        });
    }
}

const app = new App();
