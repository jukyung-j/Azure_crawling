import os
from bs4 import BeautifulSoup
import urllib.request as req

def naver():
    url = "https://finance.naver.com/marketindex/"

    res = req.urlopen(url)
    soup = BeautifulSoup(res,"html.parser", from_encoding='euc-kr')

    name_nation = soup.select('h3.h_lst > span.blind')
    name_price = soup.select('span.value')
    change = soup.select('span.change')
    blind = soup.select("div.head_info > span.blind")

    i=0
    tags = []
    try:
        tags.append(name_nation[i].text)
        tags.append(name_price[i].text)
        tags.append(change[i].text)
        tags.append(blind[i].text)

        
        keywords = [tag for tag in tags]
        
        i += 1
    except IndexError:
        pass
    return keywords