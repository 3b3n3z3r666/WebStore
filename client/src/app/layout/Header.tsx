import { AppBar, Switch, Toolbar, Typography } from "@mui/material";

interface Props {
    darkMode: boolean;
    handleThemeChange: () => void;
}

export default function Header({darkMode, handleThemeChange} : Props){
    var darkModeText = darkMode ? "dark mode": 'light mode'
    return(
        <AppBar position="static" sx={{mb: 4}}>
            <Toolbar>
                <Typography variant="h6">
                    Web Store
                </Typography>
                <Switch checked={darkMode} onChange={handleThemeChange} />
                <Typography variant="h6" sx={{color: darkMode ? 'secondary.light' : 'lightgrey' }}>
                {darkModeText}
                </Typography>
            </Toolbar>
            
        </AppBar>
    )
}