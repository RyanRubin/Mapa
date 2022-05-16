class App {
    map;
    tileLayer;
    bingMapsSource;
    offlineMapsSource;
    vectorLayer;
    vectorSource;

    initMap() {
        // window.addEventListener('contextmenu', e => e.preventDefault());

        this.bingMapsSource = new ol.source.BingMaps({
            key: 'AkGBls_Q10K9PW0jYdc1KuSPiF5qOXXICG3D9F2cT5QWLzdJsVYwgl8JYpnZg8sE',
            imagerySet: 'Aerial',
            maxZoom: 19,
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

        this.map = new ol.Map({
            target: 'map',
            layers: [this.tileLayer, this.vectorLayer],
            view: new ol.View({
                center: ol.proj.fromLonLat([-81.49559810202766, 41.469741498180625]),
                zoom: 18
            })
        });

        // this.addIcon(this.map.getView().getCenter());
        this.addCircle(this.map.getView().getCenter(), 'red');
        this.addCircle(ol.proj.fromLonLat([-81.495, 41.469]), 'green');
    }

    addIcon(coord) {
        const iconFeature = new ol.Feature({
            geometry: new ol.geom.Point(coord)
        });
        iconFeature.setStyle(new ol.style.Style({
            image: new ol.style.Icon({
                anchor: [0.5, 46],
                anchorXUnits: 'fraction',
                anchorYUnits: 'pixels',
                src: 'img/icon.png'
            })
        }));

        this.vectorSource.addFeature(iconFeature);
    }

    addCircle(coord, color) {
        const circleFeature = new ol.Feature({
            geometry: new ol.geom.Point(coord)
        });
        circleFeature.setStyle(new ol.style.Style({
            image: new ol.style.Circle({
                radius: 10,
                fill: new ol.style.Fill({
                    color: color
                })
            })
        }));

        this.vectorSource.addFeature(circleFeature);
    }
}

const app = new App();
