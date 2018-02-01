文档资料文件夹
由于mvc6  将mvc  webapi合二为一，post  请求 ，基于界限明细的方式 去加载参数
比如post 请求，在url  参数和 body  中都有标注界限
body 表单中的参数需要使用 [FromBody] 标注参数！！！ 文档参见：https://docs.microsoft.com/en-us/aspnet/core/tutorials/web-api-vsc

天猫蜘蛛防火墙：https://sec.taobao.com/query.htm?smApp=tmallsearch&smPolicy=tmallsearch-product-anti_Spider-html-checkcode&smCharset=GBK&smTag=MTgwLjc4LjI0My4xOTcsLDc2ZmMwNWI1ZGFjYjRlZGI4MjkwMTIwZGNlMmQ4NzVj&smReturn=https%3A%2F%2Flist.tmall.com%2Fsearch_product.htm%3Fq%3D%25C8%25B9%25D7%25D3%26type%3Dp%26vmarket%3D%26spm%3D875.7931836%252FB.a2227oh.d100%26from%3Dmallfp..pc_1_searchbutton&smSign=8mKNiyU9OelnJtatUe%2BMkw%3D%3D&captcha=https%3A%2F%2Fsec.taobao.com%2Fquery.htm