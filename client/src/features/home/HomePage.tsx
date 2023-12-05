import { Box, Typography } from "@mui/material";
import Slider from "react-slick";

export default function HomePage() {

    const settings = {
        dots: true,
        infinite: true,
        speed: 500,
        slidesToShow: 1,
        slidesToScroll: 1,
        display: "block",
        marginLeft: "auto",
        marginRight: "auto",
        paddingTop: 60
        // display: "flex"
    };

    return (
        <div style={{ marginTop: 40 }}>
            {/* <div style={{display:"block",justifyContent:"center"}}> */}
            <Slider {...settings} >
                <div>
                    <img src="/images/WebStore/1.jpg" alt="logo1"
                        style={{
                            display: "block", borderRadius: 50,
                            width: "25%", height: "100%", maxHeight: 500,
                            marginLeft: "auto", marginRight: "auto"
                        }} />
                </div>
                <div>
                    <img src="/images/WebStore/2.jpg" alt="logo2"
                        style={{
                            display: "block", borderRadius: 50,
                            width: "25%", height: "100%", maxHeight: 500,
                            marginLeft: "auto", marginRight: "auto"
                        }} />
                </div>
                <div>
                    <img src="/images/WebStore/3.jpg" alt="logo3"
                        style={{
                            display: "block", borderRadius: 50,
                            width: "25%", height: "100%", maxHeight: 500,
                            marginLeft: "auto", marginRight: "auto"
                        }} />
                </div>
            </Slider>
            {/* </div> */}
            <Box display="flex" justifyContent="center" sx={{ p: 4 }}>
                <Typography variant="h1">
                    Welcome to WebStore!
                </Typography>
            </Box>
        </div>
    )
}