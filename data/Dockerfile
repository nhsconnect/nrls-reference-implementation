FROM mongo:3.6

WORKDIR /data
ADD defaultdata defaultdata
ADD cmds cmds

RUN chmod +x /data/cmds/run.sh
RUN chmod +x /data/cmds/set_mongodb_users.sh
RUN chmod +x /data/cmds/set_mongodb_data.sh

CMD ["/data/cmds/run.sh"]