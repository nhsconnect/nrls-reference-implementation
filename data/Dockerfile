FROM mongo:3.6

WORKDIR /nrlsdata
ADD defaultdata defaultdata
ADD cmds cmds

RUN chmod +x /nrlsdata/cmds/run.sh
RUN chmod +x /nrlsdata/cmds/set_mongodb_users.sh
RUN chmod +x /nrlsdata/cmds/set_mongodb_data.sh

CMD ["/nrlsdata/cmds/run.sh"]