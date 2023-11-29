import { ShoppingCart } from "@mui/icons-material";
import { AppBar, Badge, Box, IconButton, List, ListItem, Switch, Toolbar, Typography } from "@mui/material";
import { Link, NavLink } from "react-router-dom";
import { useAppSelector } from "../store/configureStore";
import SignedInMenu from "./SignedInMenu";


const midLinks = [
    {title: 'catalog', path : '/catalog'},
    {title: 'about', path : '/about'},
    {title: 'contact', path : '/contact'},
]

const rightLinks = [
    {title: 'login', path : '/login'},
    {title: 'register', path : '/register'},
]

interface Props {
    darkMode: boolean;
    handleThemeChange: () => void;
}

const navStyles = {
    color:'inherit',
    textDecoration:'none', 
    typography:'h6', 
    '&:hover':{color:'grey.500'},
    '&.active': {color: 'text.secondary'}
}

export default function Header({darkMode, handleThemeChange} : Props){
    const darkModeText = darkMode ? "DARK MODE": "LIGHT MODE"
    const {basket} = useAppSelector(state => state.basket);
    const {user} = useAppSelector(state => state.account)
    
    const itemCount = basket?.items.reduce((sum, item) => sum + item.quantity, 0)
    return(
        <AppBar position="static" sx={{}}>
            <Toolbar sx={{display:'flex', justifyContent:'space-between', alignItems:'center'}}>

                <Box display='flex' alignItems='center'> 
                    <Typography     
                        variant="h6" 
                        component={NavLink} to='/' 
                        sx={navStyles}
                    >
                        WEB STORE
                    </Typography>
                        
                        <Switch checked={darkMode} onChange={handleThemeChange} />
                        <Typography 
                            variant="h6" 
                            sx={{color: darkMode ? 'secondary.light' : 'eaeaea', 
                            fontSize: 8 ,
                            textAlign:'center',
                            padding:1
                            }}
                            >
                            {darkModeText}
                        </Typography>
                </Box>
                
                
                <List sx={{display: 'flex'}}>
                    {midLinks.map(({title, path}) => (
                        <ListItem 
                            component={NavLink} to={path} key={path} 
                            sx={navStyles}
                            >
                            {title.toUpperCase()}
                        </ListItem>
                    ))}
                </List>
                
                <Box display="flex" alignItems="center">
                    <IconButton component={Link} to="/basket" size='large' edge='start' color='inherit' sx={navStyles}>
                    <Badge badgeContent={itemCount} color='secondary'>
                        <ShoppingCart sx={{}}/>
                        </Badge>
                    </IconButton>
                        {user ? (
                            <SignedInMenu />
                        ) : (
                            <List sx={{display: 'flex'}}>
                                {rightLinks.map(({title, path}) => (
                                <ListItem 
                                    component={NavLink} to={path} key={path} 
                                    sx={navStyles}>
                                    {title.toUpperCase()}
                                </ListItem>
                            ))}
                        </List>
                        )}
                </Box>                
            </Toolbar>
        </AppBar>
    )
}